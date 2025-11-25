using System.Data;
using Npgsql;
using Cafeteria.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IDbConnection>(provider => new NpgsqlConnection(builder.Configuration["DB_CONN"]));
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IFoodOptionService, FoodOptionService>();
builder.Services.AddScoped<IFoodTypeService, FoodTypeService>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
