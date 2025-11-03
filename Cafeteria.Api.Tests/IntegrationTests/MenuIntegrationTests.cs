using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Cafeteria.Shared.DTOs;
using Dapper;

namespace Cafeteria.Api.Tests.IntegrationTests;

public class MenuIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public MenuIntegrationTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        SetupDatabase();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnection));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddScoped<IDbConnection>(_ => _connection);
                });
            });

        _client = _factory.CreateClient();
    }

    private void SetupDatabase()
    {
        var createTableSql = @"
            CREATE TABLE cafeteria_location (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                location_name TEXT NOT NULL,
                location_description TEXT NOT NULL,
                image_url TEXT
            )";
        
        _connection.Execute(createTableSql);
    }

    [Fact]
    public async Task GetAllLocations_ReturnsLocationData()
    {
        var insertSql = @"
            INSERT INTO cafeteria_location (location_name, location_description, image_url)
            VALUES (@LocationName, @LocationDescription, @ImageUrl)";
        
        _connection.Execute(insertSql, new
        {
            LocationName = "Main Cafeteria",
            LocationDescription = "The main dining hall",
            ImageUrl = "https://example.com/main.jpg"
        });

        _connection.Execute(insertSql, new
        {
            LocationName = "Food Court",
            LocationDescription = "Quick service options",
            ImageUrl = "https://example.com/court.jpg"
        });

        var response = await _client.GetAsync("/api/menu/locations");
        
        response.EnsureSuccessStatusCode();
        
        var locations = await response.Content.ReadFromJsonAsync<List<LocationDto>>();
        
        Assert.NotNull(locations);
        Assert.Equal(2, locations.Count);
        
        Assert.Equal("Main Cafeteria", locations[0].LocationName);
        Assert.Equal("The main dining hall", locations[0].LocationDescription);
        Assert.Equal("https://example.com/main.jpg", locations[0].ImageUrl);
        
        Assert.Equal("Food Court", locations[1].LocationName);
        Assert.Equal("Quick service options", locations[1].LocationDescription);
        Assert.Equal("https://example.com/court.jpg", locations[1].ImageUrl);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _client?.Dispose();
        _factory?.Dispose();
    }
}
