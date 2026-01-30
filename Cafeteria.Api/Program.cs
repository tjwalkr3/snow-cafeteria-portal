using System.Data;
using Npgsql;
using Cafeteria.Api.Services.Drinks;
using Cafeteria.Api.Services.Entrees;
using Cafeteria.Api.Services.FoodOptionTypes;
using Cafeteria.Api.Services.Orders;
using Cafeteria.Api.Services.Sides;
using Cafeteria.Api.Services.Stations;
using Cafeteria.Api.Services.FoodOptions;
using Cafeteria.Api.Services.Locations;
using Cafeteria.Api.Services.OptionOptionTypes;
using Cafeteria.Api.Services.Swipes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add PostgreSQL connection
builder.AddNpgsqlDataSource("cafeteria");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddControllers();
builder.Services.AddScoped<IDbConnection>(provider => provider.GetRequiredService<NpgsqlDataSource>().CreateConnection());
builder.Services.AddScoped<IFoodOptionService, FoodOptionService>();
builder.Services.AddScoped<IFoodOptionTypeService, FoodOptionTypeService>();
builder.Services.AddScoped<IOptionOptionTypeService, OptionOptionTypeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<IEntreeService, EntreeService>();
builder.Services.AddScoped<ISideService, SideService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISwipeService, SwipeService>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
