using Cafeteria.Customer.Components;
using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Customer.Components.Pages.OrderHistory;
using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;
using Cafeteria.Customer.Components.Pages.Stations.Strategies;
using Cafeteria.Customer.Services.Auth;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Customer.Services.Printer;
using Cafeteria.Customer.Services.Storage;
using Cafeteria.Customer.Services.Swipe;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

// Register HTTP client for API calls with authentication
builder.Services.AddHttpClient<IHttpClientAuth, HttpClientAuth>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Api Data Services
builder.Services.AddScoped<IApiMenuService, ApiMenuService>();
builder.Services.AddScoped<IApiOrderService, ApiOrderService>();
builder.Services.AddScoped<IApiSwipeService, ApiSwipeService>();
builder.Services.AddScoped<IPrinterService, PrinterService>();

builder.Services.AddHttpClient<IAuthService, AuthService>();

// Register view models
builder.Services.AddScoped<ILocationSelectVM, LocationSelectVM>();
builder.Services.AddScoped<IStationSelectVM, StationSelectVM>();
builder.Services.AddScoped<IPlaceOrderVM, PlaceOrderVM>();
builder.Services.AddScoped<IOrderHistoryVM, OrderHistoryVM>();

// Register generic station services
builder.Services.AddSingleton<IStationConfigurationProvider, StationConfigurationProvider>();
builder.Services.AddScoped<ISelectionStrategyFactory, SelectionStrategyFactory>();
builder.Services.AddScoped<IGenericSwipeVM, GenericSwipeVM>();

// Register cart service and storage wrapper
builder.Services.AddScoped<IStorageWrapper, StorageWrapper>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddSingleton<CartNotificationService>();

// Add authentication services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/signout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/auth/signin", async (HttpContext httpContext, string token, string? returnUrl) =>
{
    try
    {
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var sessionData = JsonSerializer.Deserialize<JsonElement>(json);

        var userInfo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
            sessionData.GetProperty("UserInfo").GetRawText());

        if (userInfo == null)
        {
            return Results.Redirect("/signin?error=invalid_session");
        }

        var accessToken = sessionData.GetProperty("AccessToken").GetString();
        var refreshToken = sessionData.TryGetProperty("RefreshToken", out var rt) ? rt.GetString() : string.Empty;
        var tokenType = sessionData.TryGetProperty("TokenType", out var tt) ? tt.GetString() : "Bearer";
        var expiresIn = sessionData.TryGetProperty("ExpiresIn", out var ei) ? ei.GetInt32() : 300;

        var claims = BuildClaimsFromUserInfo(userInfo);

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
        };

        authProperties.StoreTokens([
            new AuthenticationToken { Name = "access_token", Value = accessToken! },
            new AuthenticationToken { Name = "refresh_token", Value = refreshToken ?? string.Empty },
            new AuthenticationToken { Name = "token_type", Value = tokenType ?? "Bearer" }
        ]);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        return Results.Redirect(returnUrl ?? "/");
    }
    catch
    {
        return Results.Redirect("/signin?error=invalid_session");
    }
});

app.MapGet("/signout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    httpContext.Response.Redirect("/");
});

app.MapGet("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    httpContext.Response.Redirect("/");
});

app.Run();

List<Claim> BuildClaimsFromUserInfo(Dictionary<string, JsonElement> userInfo)
{
    var claims = new List<Claim>();

    foreach (var claim in userInfo)
    {
        if (claim.Value.ValueKind == JsonValueKind.String)
        {
            claims.Add(new Claim(claim.Key, claim.Value.GetString()!));
        }
        else if (claim.Value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in claim.Value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    claims.Add(new Claim(claim.Key, item.GetString()!));
                }
            }
        }
    }

    return claims;
}
