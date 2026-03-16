using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Npgsql;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class IconIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public IconIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
        _connection = _fixture.GetConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    [Fact]
    public async Task GetAllIcons_ReturnsAllIcons()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/icon");
        response.EnsureSuccessStatusCode();
        var icons = await response.Content.ReadFromJsonAsync<List<IconDto>>();

        Assert.NotNull(icons);
        Assert.True(icons.Count > 0);
        // Verify basic structure
        Assert.All(icons, icon => Assert.NotNull(icon.IconName));
    }
}
