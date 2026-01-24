using Cafeteria.Customer.Components;
using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;
using Cafeteria.Customer.Components.Pages.Stations.Strategies;
using Cafeteria.Customer.Services;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Api Data Service with service discovery
builder.Services.AddHttpClient<IApiMenuService, ApiMenuService>(client =>
{
    // Use configuration that works in both Aspire (via env var) and Kubernetes (via appsettings)
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IApiOrderService, ApiOrderService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://api/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register view models
builder.Services.AddScoped<ILocationSelectVM, LocationSelectVM>();
builder.Services.AddScoped<IStationSelectVM, StationSelectVM>();
builder.Services.AddScoped<IPlaceOrderVM, PlaceOrderVM>();

// Register generic station services
builder.Services.AddSingleton<IStationConfigurationProvider, StationConfigurationProvider>();
builder.Services.AddScoped<ISelectionStrategyFactory, SelectionStrategyFactory>();
builder.Services.AddScoped<IGenericSwipeVM, GenericSwipeVM>();

// Register cart service and storage wrapper
builder.Services.AddScoped<IStorageWrapper, StorageWrapper>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddSingleton<CartNotificationService>();

// Register printer services
builder.Services.AddSingleton<IPrinterService, PrinterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Skip HTTPS redirection in Docker/containerized environments
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapDefaultEndpoints();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
