using Cafeteria.Customer.Components;
using Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Customer.Components.ViewModelInterfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IItemSelectVM, ItemSelectVM>();
builder.Services.AddScoped<ILocationSelectVM, LocationSelectVM>();
builder.Services.AddScoped<IStationSelectVM, StationSelectVM>();
builder.Services.AddScoped<IFoodItemBuilderVM, FoodItemBuilderVM>();
builder.Services.AddScoped<IPlaceOrderVM, PlaceOrderVM>();

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
