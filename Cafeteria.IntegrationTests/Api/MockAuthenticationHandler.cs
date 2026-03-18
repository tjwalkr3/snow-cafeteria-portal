using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cafeteria.IntegrationTests.Api;

/// <summary>
/// Mock authentication handler for integration tests.
/// This handler automatically authenticates all requests with a test user.
/// </summary>
public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "TestScheme";
    public const string TestEmailHeader = "X-Test-Email";
    public const string TestNameHeader = "X-Test-Name";
    public const string TestRoleHeader = "X-Test-Role";

    public MockAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var email = Request.Headers[TestEmailHeader].FirstOrDefault() ?? "test@example.com";
        var name = Request.Headers[TestNameHeader].FirstOrDefault() ?? "Test User";
        var role = Request.Headers[TestRoleHeader].FirstOrDefault() ?? "admin";

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, name),
            new Claim("name", name),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Email, email),
            new Claim("email", email),
            new Claim("preferred_username", email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
