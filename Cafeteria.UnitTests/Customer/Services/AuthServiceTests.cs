using Cafeteria.Shared.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

namespace Cafeteria.UnitTests.Customer.Services;

public class AuthServiceTests
{
    [Fact]
    public void BuildClaimsFromUserInfo_ReturnsEmptyList_WhenUserInfoIsEmpty()
    {
        var userInfo = new Dictionary<string, JsonElement>();

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Empty(result);
    }

    [Fact]
    public void BuildClaimsFromUserInfo_AddsSingleClaim_WhenUserInfoContainsStringValue()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Single(result);
        Assert.Equal(System.Security.Claims.ClaimTypes.Email, result[0].Type);
        Assert.Equal("user@example.com", result[0].Value);
    }

    [Fact]
    public void BuildClaimsFromUserInfo_AddsMultipleClaims_WhenUserInfoContainsMultipleStringValues()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") },
            { "name", JsonSerializer.SerializeToElement("John Doe") },
            { "sub", JsonSerializer.SerializeToElement("12345") }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "user@example.com");
        Assert.Contains(result, c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == "John Doe");
        Assert.Contains(result, c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == "12345");
    }

    [Fact]
    public void BuildClaimsFromUserInfo_AddsMultipleClaims_WhenUserInfoContainsArrayValue()
    {
        var roles = new[] { "admin", "user" };
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "roles", JsonSerializer.SerializeToElement(roles) }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Type == "roles" && c.Value == "admin");
        Assert.Contains(result, c => c.Type == "roles" && c.Value == "user");
    }

    [Fact]
    public void BuildClaimsFromUserInfo_IgnoresNonStringArrayElements()
    {
        var mixedArray = JsonSerializer.SerializeToElement(new object[] { "valid", 123, true });
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "mixed", mixedArray }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Single(result);
        Assert.Equal("mixed", result[0].Type);
        Assert.Equal("valid", result[0].Value);
    }

    [Fact]
    public void BuildClaimsFromUserInfo_IgnoresNonStringAndNonArrayValues()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") },
            { "age", JsonSerializer.SerializeToElement(30) },
            { "active", JsonSerializer.SerializeToElement(true) }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Single(result);
        Assert.Equal(System.Security.Claims.ClaimTypes.Email, result[0].Type);
        Assert.Equal("user@example.com", result[0].Value);
    }

    [Fact]
    public void BuildClaimsFromUserInfo_HandlesMixedStringAndArrayValues()
    {
        var roles = new[] { "admin", "user" };
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") },
            { "name", JsonSerializer.SerializeToElement("John Doe") },
            { "roles", JsonSerializer.SerializeToElement(roles) }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Equal(4, result.Count);
        Assert.Contains(result, c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "user@example.com");
        Assert.Contains(result, c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == "John Doe");
        Assert.Contains(result, c => c.Type == "roles" && c.Value == "admin");
        Assert.Contains(result, c => c.Type == "roles" && c.Value == "user");
    }

    [Fact]
    public void BuildClaimsFromUserInfo_HandlesEmptyArray()
    {
        var emptyArray = Array.Empty<string>();
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "roles", JsonSerializer.SerializeToElement(emptyArray) }
        };

        var result = AuthService.BuildClaimsFromUserInfo(userInfo);

        Assert.Empty(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsValidAuthData_WhenTokenIsValid()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") },
            { "name", JsonSerializer.SerializeToElement("John Doe") }
        };

        var sessionData = new
        {
            UserInfo = userInfo,
            AccessToken = "access-token-123",
            RefreshToken = "refresh-token-456",
            TokenType = "Bearer",
            ExpiresIn = 3600
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = new DateTimeOffset(2026, 2, 7, 10, 0, 0, TimeSpan.Zero);

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.NotNull(result);
        var (claimsPrincipal, authProperties) = result.Value;

        Assert.NotNull(claimsPrincipal);
        Assert.Equal(2, claimsPrincipal.Claims.Count());
        Assert.Contains(claimsPrincipal.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "user@example.com");
        Assert.Contains(claimsPrincipal.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == "John Doe");

        Assert.NotNull(authProperties);
        Assert.True(authProperties.IsPersistent);
        Assert.Equal(currentTime.AddSeconds(3600), authProperties.ExpiresUtc);

        var tokens = authProperties.GetTokens().ToList();
        Assert.Equal(3, tokens.Count);
        Assert.Contains(tokens, t => t.Name == "access_token" && t.Value == "access-token-123");
        Assert.Contains(tokens, t => t.Name == "refresh_token" && t.Value == "refresh-token-456");
        Assert.Contains(tokens, t => t.Name == "token_type" && t.Value == "Bearer");
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsValidAuthData_WhenOptionalFieldsAreMissing()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") }
        };

        var sessionData = new
        {
            UserInfo = userInfo,
            AccessToken = "access-token-123"
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = new DateTimeOffset(2026, 2, 7, 10, 0, 0, TimeSpan.Zero);

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.NotNull(result);
        var (claimsPrincipal, authProperties) = result.Value;

        Assert.NotNull(claimsPrincipal);
        Assert.Single(claimsPrincipal.Claims);

        Assert.NotNull(authProperties);
        Assert.Equal(currentTime.AddHours(1), authProperties.ExpiresUtc);

        var tokens = authProperties.GetTokens().ToList();
        Assert.Contains(tokens, t => t.Name == "access_token" && t.Value == "access-token-123");
        Assert.Contains(tokens, t => t.Name == "refresh_token" && t.Value == string.Empty);
        Assert.Contains(tokens, t => t.Name == "token_type" && t.Value == "Bearer");
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsNull_WhenTokenIsInvalidBase64()
    {
        var token = "invalid-base64!!!";
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.Null(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsNull_WhenTokenIsNotValidJson()
    {
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("not valid json"));
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.Null(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsNull_WhenUserInfoIsMissing()
    {
        var sessionData = new
        {
            AccessToken = "access-token-123"
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.Null(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsNull_WhenUserInfoIsNull()
    {
        var json = "{\"UserInfo\":null,\"AccessToken\":\"access-token-123\"}";
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.Null(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_ReturnsNull_WhenAccessTokenIsMissing()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") }
        };

        var sessionData = new
        {
            UserInfo = userInfo
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.Null(result);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_UsesConfiguredCookieLifetime_ForExpiresUtc()
    {
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") }
        };

        var sessionData = new
        {
            UserInfo = userInfo,
            AccessToken = "access-token-123",
            ExpiresIn = 7200
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.NotNull(result);
        var (_, authProperties) = result.Value;

        Assert.Equal(currentTime.AddHours(1), authProperties.ExpiresUtc);
    }

    [Fact]
    public void ParseTokenAndCreateAuthData_HandlesUserInfoWithArrayValues()
    {
        var roles = new[] { "admin", "user" };
        var userInfo = new Dictionary<string, JsonElement>
        {
            { "email", JsonSerializer.SerializeToElement("user@example.com") },
            { "roles", JsonSerializer.SerializeToElement(roles) }
        };

        var sessionData = new
        {
            UserInfo = userInfo,
            AccessToken = "access-token-123"
        };

        var json = JsonSerializer.Serialize(sessionData);
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        var currentTime = DateTimeOffset.UtcNow;

        var result = AuthService.ParseTokenAndCreateAuthData(token, currentTime);

        Assert.NotNull(result);
        var (claimsPrincipal, _) = result.Value;

        Assert.Equal(3, claimsPrincipal.Claims.Count());
        Assert.Contains(claimsPrincipal.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == "user@example.com");
        Assert.Contains(claimsPrincipal.Claims, c => c.Type == "roles" && c.Value == "admin");
        Assert.Contains(claimsPrincipal.Claims, c => c.Type == "roles" && c.Value == "user");
    }
}
