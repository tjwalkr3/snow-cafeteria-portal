using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using static Cafeteria.Api.Tests.IntegrationTests.DBSql;

namespace Cafeteria.Api.Tests.IntegrationTests;

public class MenuIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public MenuIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("cafeteria")
            .WithUsername("cafeteria_admin")
            .WithPassword("SnowCafe")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await _connection.OpenAsync();
        await _connection.ExecuteAsync(SqlData);

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

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetAllLocations_ReturnsLocationData()
    {
        // Arrange
        var insertSql = @"
            INSERT INTO cafeteria.cafeteria_location (location_name, location_description, image_url)
            VALUES (@LocationName, @LocationDescription, @ImageUrl)";
        List<LocationDto> locationsBefore = [
            new() {
                Id = 1,
                LocationName = "Badger Den",
                LocationDescription = "Located on the main floor of the Greenwood Student Center",
                ImageUrl = "https://picsum.photos/id/292/300/200"
            },
            new() {
                Id = 2,
                LocationName = "Busters Bistro",
                LocationDescription = "Located on the main floor of the Karen H Huntsman Library",
                ImageUrl = "https://picsum.photos/id/326/300/200"
            }
        ];
        _connection.Execute(insertSql, locationsBefore[0]);
        _connection.Execute(insertSql, locationsBefore[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/locations");
        response.EnsureSuccessStatusCode();
        var locationsAfter = await response.Content.ReadFromJsonAsync<List<LocationDto>>();
        
        // Assert
        Assert.NotNull(locationsAfter);
        Assert.Equal(2, locationsAfter.Count);
        
        Assert.Equal(locationsBefore[0].Id, locationsAfter[0].Id);
        Assert.Equal(locationsBefore[0].LocationName, locationsAfter[0].LocationName);
        Assert.Equal(locationsBefore[0].LocationDescription, locationsAfter[0].LocationDescription);
        Assert.Equal(locationsBefore[0].ImageUrl, locationsAfter[0].ImageUrl);
        
        Assert.Equal(locationsBefore[1].Id, locationsAfter[1].Id);
        Assert.Equal(locationsBefore[1].LocationName, locationsAfter[1].LocationName);
        Assert.Equal(locationsBefore[1].LocationDescription, locationsAfter[1].LocationDescription);
        Assert.Equal(locationsBefore[1].ImageUrl, locationsAfter[1].ImageUrl);
    }

    [Fact]
    public async Task GetStationsByLocation_ReturnsStationData()
    {
        // Arrange
        var insertLocationSql = @"
            INSERT INTO cafeteria.cafeteria_location (location_name, location_description, image_url)
            VALUES (@LocationName, @LocationDescription, @ImageUrl)";
        
        var locationDto = new LocationDto
        {
            Id = 1,
            LocationName = "Badger Den",
            LocationDescription = "Located on the main floor of the Greenwood Student Center",
            ImageUrl = "https://picsum.photos/id/292/300/200"
        };
        _connection.Execute(insertLocationSql, locationDto);

        var insertStationSql = @"
            INSERT INTO cafeteria.station (location_id, station_name, station_description)
            VALUES (@LocationId, @StationName, @StationDescription)";
        
        List<StationDto> stationsBefore = [
            new() {
                Id = 1,
                LocationId = 1,
                StationName = "Sandwich Station",
                StationDescription = "Fresh made-to-order sandwiches"
            },
            new() {
                Id = 2,
                LocationId = 1,
                StationName = "Salad Bar",
                StationDescription = "Fresh salads and healthy options"
            }
        ];
        _connection.Execute(insertStationSql, stationsBefore[0]);
        _connection.Execute(insertStationSql, stationsBefore[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/stations/location/1");
        response.EnsureSuccessStatusCode();
        var stationsAfter = await response.Content.ReadFromJsonAsync<List<StationDto>>();
        
        // Assert
        Assert.NotNull(stationsAfter);
        Assert.Equal(2, stationsAfter.Count);
        
        Assert.Equal(stationsBefore[0].Id, stationsAfter[0].Id);
        Assert.Equal(stationsBefore[0].LocationId, stationsAfter[0].LocationId);
        Assert.Equal(stationsBefore[0].StationName, stationsAfter[0].StationName);
        Assert.Equal(stationsBefore[0].StationDescription, stationsAfter[0].StationDescription);
        
        Assert.Equal(stationsBefore[1].Id, stationsAfter[1].Id);
        Assert.Equal(stationsBefore[1].LocationId, stationsAfter[1].LocationId);
        Assert.Equal(stationsBefore[1].StationName, stationsAfter[1].StationName);
        Assert.Equal(stationsBefore[1].StationDescription, stationsAfter[1].StationDescription);
    }
}
