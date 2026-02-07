using Cafeteria.Management.Components;
using Cafeteria.Management.Components.Pages.Drink;
using Cafeteria.Management.Components.Pages.Entree;
using Cafeteria.Management.Components.Pages.FoodOption;
using Cafeteria.Management.Components.Pages.FoodType;
using Cafeteria.Management.Components.Pages.LocationAndStation;
using Cafeteria.Management.Components.Pages.Side;
using Cafeteria.Management.Services.Auth;
using Cafeteria.Management.Services.Customers;
using Cafeteria.Management.Services.Drinks;
using Cafeteria.Management.Services.Entrees;
using Cafeteria.Management.Services.FoodOptions;
using Cafeteria.Management.Services.FoodOptionTypes;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.OptionOptionTypes;
using Cafeteria.Management.Services.Sides;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Orders;
using Cafeteria.Management.Components.Pages.Order;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";

builder.Services.AddHttpClient<IHttpClientAuth, HttpClientAuth>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add HttpClient with BaseAddress for Blazor components
builder.Services.AddHttpClient("default")
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
    });

builder.Services.AddHttpClient<IAuthService, AuthService>();

builder.Services.AddScoped<IFoodOptionService, FoodOptionService>();
builder.Services.AddScoped<IFoodOptionTypeService, FoodOptionTypeService>();
builder.Services.AddScoped<IOptionOptionTypeService, OptionOptionTypeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<ISideService, SideService>();
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IEntreeService, EntreeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


// Register ViewModels
builder.Services.AddScoped<ILocationAndStationVM, LocationAndStationVM>();
builder.Services.AddScoped<IEntreeVM, EntreeVM>();
builder.Services.AddScoped<IDrinkVM, DrinkVM>();
builder.Services.AddScoped<ICreateOrEditDrinkVM, CreateOrEditDrinkVM>();
builder.Services.AddScoped<ISideVM, SideVM>();
builder.Services.AddScoped<ICreateOrEditSideVM, CreateOrEditSideVM>();
builder.Services.AddScoped<IFoodOptionVM, FoodOptionVM>();
builder.Services.AddScoped<IFoodOptionModalVM, FoodOptionModalVM>();
builder.Services.AddScoped<IFoodTypeVM, FoodTypeVM>();
builder.Services.AddScoped<IFoodTypeModalVM, FoodTypeModalVM>();
builder.Services.AddScoped<IOptionOptionTypeVM, OptionOptionTypeVM>();
builder.Services.AddScoped<IOrderVM, OrderVM>();


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
    return Results.Redirect("/signin");
});

app.Run();

List<Claim> BuildClaimsFromUserInfo(Dictionary<string, JsonElement> userInfo)
{
    var claims = new List<Claim>();

    // Map OIDC claims to standard .NET claim types
    var claimMappings = new Dictionary<string, string>
    {
        { "sub", ClaimTypes.NameIdentifier },
        { "preferred_username", ClaimTypes.Name },
        { "name", ClaimTypes.Name },
        { "email", ClaimTypes.Email },
        { "given_name", ClaimTypes.GivenName },
        { "family_name", ClaimTypes.Surname }
    };

    foreach (var claim in userInfo)
    {
        // Determine the claim type to use
        var claimType = claimMappings.ContainsKey(claim.Key) ? claimMappings[claim.Key] : claim.Key;

        if (claim.Value.ValueKind == JsonValueKind.String)
        {
            var value = claim.Value.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                claims.Add(new Claim(claimType, value));
            }
        }
        else if (claim.Value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in claim.Value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var value = item.GetString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        claims.Add(new Claim(claimType, value));
                    }
                }
            }
        }
    }

    return claims;
}
