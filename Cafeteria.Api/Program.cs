using System.Data;
using Npgsql;
using Cafeteria.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add PostgreSQL connection
builder.AddNpgsqlDataSource("cafeteria");

builder.Services.AddControllers();
builder.Services.AddScoped<IDbConnection>(provider => provider.GetRequiredService<NpgsqlDataSource>().CreateConnection());
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

//app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
