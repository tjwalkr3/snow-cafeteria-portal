var builder = DistributedApplication.CreateBuilder(args);
var repoRoot = Path.GetFullPath(Path.Combine(builder.AppHostDirectory!, ".."));
var realmPath = Path.Combine(repoRoot, "AppRealm.json");

const string postgresPassword = "SnowCafe";
const string postgresUser = "cafeteria_admin";
const string postgresDb = "cafeteria";
const int postgresPort = 5432;
const int keycloakPort = 8080;
const string keycloakHost = "keycloak";
const string keycloakRealm = "AppRealm";

var postgres = builder.AddContainer("postgres", "postgres", "17-trixie")
    .WithEnvironment("POSTGRES_PASSWORD", postgresPassword)
    .WithEnvironment("POSTGRES_USER", postgresUser)
    .WithEnvironment("POSTGRES_DB", postgresDb)
    .WithBindMount("../init.sql", "/docker-entrypoint-initdb.d/init.sql")
    .WithEndpoint(port: postgresPort, targetPort: postgresPort, name: "postgres")
    .WithLifetime(ContainerLifetime.Session);

builder.AddContainer(keycloakHost, "keycloak/keycloak", "26.0")
    .WithBindMount(realmPath, "/opt/keycloak/data/import/AppRealm.json")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
    .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "password123")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
    .WithHttpEndpoint(port: keycloakPort, targetPort: keycloakPort, name: "http")
    .WithArgs("start-dev", "--import-realm")
    .WithLifetime(ContainerLifetime.Session);

var connectionString = $"Host=localhost;Port={postgresPort};Database={postgresDb};Username={postgresUser};Password={postgresPassword}";
var keycloakAuthority = $"http://localhost:{keycloakPort}/realms/{keycloakRealm}";

var api = builder.AddProject<Projects.Cafeteria_Api>("api")
    .WithEnvironment("ConnectionStrings__cafeteria", connectionString)
    .WithEnvironment("Keycloak__Authority", keycloakAuthority);

builder.AddProject<Projects.Cafeteria_Customer>("customer")
    .WithReference(api)
    .WithEnvironment("ApiBaseUrl", "http://api/api/")
    .WithEnvironment("Keycloak__Authority", keycloakAuthority)
    .WithEnvironment("Keycloak__ClientId", "ham-exam")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.Cafeteria_Management>("management")
    .WithReference(api)
    .WithEnvironment("OpenIDConnectSettings__Authority", keycloakAuthority)
    .WithEnvironment("OpenIDConnectSettings__ClientId", "cafeteria")
    .WithEnvironment("OpenIDConnectSettings__ClientSecret", string.Empty)
    .WithExternalHttpEndpoints();

builder.Build().Run();
