using Cafeteria.Management.Components;
using Cafeteria.Management.Components.Pages.Drink;
using Cafeteria.Management.Components.Pages.Entree;
using Cafeteria.Management.Components.Pages.LocationAndStation;
using Cafeteria.Management.Components.Pages.Side;
using Cafeteria.Management.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IHttpClientAuth, HttpClientAuth>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFoodOptionService, FoodOptionService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFoodTypeService, FoodTypeService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFoodOptionService, FoodOptionService>(client =>
{
    // Use configuration that works in both Aspire (via env var) and Kubernetes (via appsettings)
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IFoodTypeService, FoodTypeService>(client =>
{
    // Use configuration that works in both Aspire (via env var) and Kubernetes (via appsettings)
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ILocationService, LocationService>(client =>
{
    // Use configuration that works in both Aspire (via env var) and Kubernetes (via appsettings)
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IStationService, StationService>(client =>
{
    // Use configuration that works in both Aspire (via env var) and Kubernetes (via appsettings)
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register ViewModels
builder.Services.AddScoped<ILocationAndStationVM, LocationAndStationVM>();
builder.Services.AddScoped<EntreeVM>();
builder.Services.AddScoped<IDrinkVM, DrinkVM>();
builder.Services.AddScoped<ISideVM, SideVM>();

// Add authentication services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");

        options.Authority = oidcConfig["Authority"];
        options.ClientId = oidcConfig["ClientId"];
        options.ClientSecret = oidcConfig["ClientSecret"];

        // Take this out in prod
        options.RequireHttpsMetadata = false;

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.ResponseType = OpenIdConnectResponseType.Code;

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.MapInboundClaims = false;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
        options.TokenValidationParameters.RoleClaimType = "roles";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = returnUrl
    });
});

app.MapGet("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});

app.Run();
