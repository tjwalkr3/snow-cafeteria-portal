using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;

namespace Cafeteria.IntegrationTests.Api;

/// <summary>
/// Shared database fixture that creates a single Docker container for all integration tests.
/// This fixture is shared across all test classes in the same collection.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;
    private NpgsqlConnection? _connection;

    public DatabaseFixture()
    {
        _postgresContainer = new PostgreSqlBuilder("postgres:18-alpine")
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

        // Create tables and insert sample data
        await _connection.ExecuteAsync(DBSql.SqlDataWithSampleData);

        var connectionString = _postgresContainer.GetConnectionString();
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace database connection
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IDbConnection)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddScoped<IDbConnection>(_ =>
                {
                    var conn = new NpgsqlConnection(connectionString);
                    conn.Open();
                    return conn;
                });

                // Add mock authentication
                services.AddAuthentication(MockAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.AuthenticationScheme,
                        options => { });
            });
        });

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        Factory?.Dispose();
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await _postgresContainer.DisposeAsync();
    }

    /// <summary>
    /// Gets a new database connection for direct database operations in tests.
    /// The caller is responsible for disposing this connection.
    /// </summary>
    public NpgsqlConnection GetConnection()
    {
        var conn = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        conn.Open();
        return conn;
    }
}

/// <summary>
/// Collection definition for tests that share the database fixture.
/// All test classes that use [Collection("Database")] will share the same database container.
/// </summary>
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
