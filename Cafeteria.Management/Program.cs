using Cafeteria.Management.Components;
using Cafeteria.Management.Components.Pages.Drink;
using Cafeteria.Management.Components.Pages.Entree;
using Cafeteria.Management.Components.Pages.FoodOption;
using Cafeteria.Management.Components.Pages.FoodType;
using Cafeteria.Management.Components.Pages.LocationAndStation;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Components.Pages.LocationAndStation.Station;
using Cafeteria.Management.Components.Pages.Side;
using Cafeteria.Shared.Services.Auth;
using Cafeteria.Shared.Services.Customer;
using Cafeteria.Shared.Services.Portal;
using Cafeteria.Management.Services.Customers;
using Cafeteria.Management.Services.Drinks;
using Cafeteria.Management.Services.Entrees;
using Cafeteria.Management.Services.FoodOptions;
using Cafeteria.Management.Services.FoodOptionTypes;
using Cafeteria.Management.Services.Icons;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.OptionOptionTypes;
using Cafeteria.Management.Services.SchedulingExceptions;
using Cafeteria.Management.Services.Sides;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Orders;
using Cafeteria.Management.Components.Pages.Order;
using Cafeteria.Management.Components.Pages.Analytics;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Cafeteria.Shared.Controllers.AuthController).Assembly);

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

builder.Services.AddSingleton<IPortalSettings>(new PortalSettings
{
    PortalName = "Management Portal",
    SignInSubtitle = "Sign in to access the management system"
});
builder.Services.AddScoped<IIconService, IconService>();
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
builder.Services.AddScoped<ISchedulingExceptionsService, SchedulingExceptionsService>();

builder.Services.AddHttpClient<ICustomerRegistrationService, CustomerRegistrationService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});


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
builder.Services.AddScoped<IAnalyticsVM, AnalyticsVM>();
builder.Services.AddScoped<IManageLocationVM, ManageLocationVM>();
builder.Services.AddScoped<ICreateOrEditLocationVM, CreateOrEditLocationVM>();
builder.Services.AddScoped<ICreateOrEditStationVM, CreateOrEditStationVM>();


// Add authentication services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/auth/signout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Cafeteria.Shared.Components.Pages.SignIn.SignIn).Assembly);

app.MapControllers();

app.Run();
