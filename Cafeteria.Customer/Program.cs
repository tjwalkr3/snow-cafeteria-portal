using Cafeteria.Customer.Components;
using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;
using Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;
using Cafeteria.Customer.Components.Pages.Stations.GrillSwipe;
using Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;
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
    // This will be configured via service discovery from Aspire
    client.BaseAddress = new Uri("http://api/api/");
});

// Register view models
builder.Services.AddScoped<ILocationSelectVM, LocationSelectVM>();
builder.Services.AddScoped<IStationSelectVM, StationSelectVM>();
builder.Services.AddScoped<IPlaceOrderVM, PlaceOrderVM>();
builder.Services.AddScoped<IBreakfastSwipeVM, BreakfastSwipeVM>();
builder.Services.AddScoped<IDeliSwipeVM, DeliSwipeVM>();
builder.Services.AddScoped<IGrillSwipeVM, GrillSwipeVM>();
builder.Services.AddScoped<IPizzaSwipeVM, PizzaSwipeVM>();

// Register cart service and storage wrapper
builder.Services.AddScoped<IStorageWrapper, StorageWrapper>();
builder.Services.AddScoped<ICartService, CartService>();

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
