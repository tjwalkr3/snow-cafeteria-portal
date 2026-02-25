using Cafeteria.Customer.Components;
using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Customer.Components.Pages.OrderHistory;
using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.FoodBuilder;
using Cafeteria.Customer.Components.Pages.Stations.Strategies;
using Cafeteria.Shared.Services.Auth;
using Cafeteria.Shared.Services.Customer;
using Cafeteria.Shared.Services.Portal;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Customer.Services.Printer;
using Cafeteria.Customer.Services.Storage;
using Cafeteria.Customer.Services.Swipe;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Cafeteria.Shared.Controllers.AuthController).Assembly);

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

builder.Services.AddHttpClient<ICustomerRegistrationService, CustomerRegistrationService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IAuthService, AuthService>();

builder.Services.AddSingleton<IPortalSettings>(new PortalSettings
{
    PortalName = "Customer Portal",
    SignInSubtitle = "Sign in to access the customer system"
});

// Register view models
builder.Services.AddScoped<ILocationSelectVM, LocationSelectVM>();
builder.Services.AddScoped<IStationSelectVM, StationSelectVM>();
builder.Services.AddScoped<IPlaceOrderVM, PlaceOrderVM>();
builder.Services.AddScoped<IOrderHistoryVM, OrderHistoryVM>();

// Register generic station services
builder.Services.AddSingleton<IStationConfigurationProvider, StationConfigurationProvider>();
builder.Services.AddScoped<ISelectionStrategyFactory, SelectionStrategyFactory>();
builder.Services.AddScoped<IFoodBuilderVM, FoodBuilderVM>();

// Register cart service and storage wrapper
builder.Services.AddScoped<IStorageWrapper, StorageWrapper>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddSingleton<CartNotificationService>();

// Add authentication services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/signin";
        options.LogoutPath = "/auth/signout";
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
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Cafeteria.Shared.Components.Pages.SignIn.SignIn).Assembly);

app.MapControllers();

app.Run();
