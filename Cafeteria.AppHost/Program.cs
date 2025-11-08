var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL as a container resource (not using Aspire's AddPostgres)
// This gives us full control over the connection string
var postgresPassword = "SnowCafe";
var postgresUser = "cafeteria_admin";
var postgresDb = "cafeteria";

var postgres = builder.AddContainer("postgres", "postgres", "17-trixie")
    .WithEnvironment("POSTGRES_PASSWORD", postgresPassword)
    .WithEnvironment("POSTGRES_USER", postgresUser)
    .WithEnvironment("POSTGRES_DB", postgresDb)
    .WithBindMount("../init.sql", "/docker-entrypoint-initdb.d/init.sql")
    .WithEndpoint(port: 5432, targetPort: 5432, name: "postgres")
    .WithLifetime(ContainerLifetime.Persistent);

// Build the connection string using localhost since API runs on host in Aspire
// In Kubernetes, the appsettings.json value will be used instead
var connectionString = $"Host=localhost;Port=5432;Database={postgresDb};Username={postgresUser};Password={postgresPassword}";

// Add API service with explicit connection string override for Aspire
var api = builder.AddProject<Projects.Cafeteria_Api>("api")
    .WithEnvironment("ConnectionStrings__cafeteria", connectionString);

// Add Customer Blazor app with API URL override for Aspire
builder.AddProject<Projects.Cafeteria_Customer>("customer")
    .WithReference(api)
    .WithEnvironment("ApiBaseUrl", "http://api/api/")
    .WithExternalHttpEndpoints();

// Add Management Blazor app
builder.AddProject<Projects.Cafeteria_Management>("management")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
