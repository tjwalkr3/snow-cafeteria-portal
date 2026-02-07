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
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

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

        // Take this out in prod
        options.RequireHttpsMetadata = false;

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.ResponseType = OpenIdConnectResponseType.Code;

        // Disable PAR since we're using a public client
        options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

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
}

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
