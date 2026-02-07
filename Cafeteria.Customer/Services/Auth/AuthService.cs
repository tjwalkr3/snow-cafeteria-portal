using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Services.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<AuthResult> ValidateCredentialsAsync(string username, string password)
    {
        var tokenResponse = await RequestTokenAsync(username, password);

        if (!tokenResponse.Success || tokenResponse.AccessToken == null)
        {
            return CreateFailureResult(tokenResponse.ErrorMessage);
        }

        var userInfo = await FetchUserInfoAsync(tokenResponse.AccessToken);

        if (userInfo == null)
        {
            return CreateFailureResult("Failed to retrieve user information");
        }

        var sessionToken = CreateSessionToken(tokenResponse, userInfo);

        return new AuthResult
        {
            Success = true,
            SessionToken = sessionToken
        };
    }

    private string CreateSessionToken(TokenResponse tokenResponse, Dictionary<string, JsonElement> userInfo)
    {
        var sessionData = new
        {
            UserInfo = userInfo,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            TokenType = tokenResponse.TokenType,
            ExpiresIn = tokenResponse.ExpiresIn
        };

        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sessionData)));
    }

    private async Task<TokenResponse> RequestTokenAsync(string username, string password)
    {
        try
        {
            var authority = _configuration["OpenIDConnectSettings:Authority"];
            var clientId = _configuration["OpenIDConnectSettings:ClientId"];

            var tokenEndpoint = $"{authority}/protocol/openid-connect/token";

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", clientId!),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("scope", "openid profile email")
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                return CreateTokenFailureResponse();
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(content);

            return CreateTokenSuccessResponse(tokenData);
        }
        catch
        {
            return CreateTokenFailureResponse();
        }
    }

    private async Task<Dictionary<string, JsonElement>?> FetchUserInfoAsync(string accessToken)
    {
        try
        {
            var authority = _configuration["OpenIDConnectSettings:Authority"];
            var userInfoEndpoint = $"{authority}/protocol/openid-connect/userinfo";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(userInfoEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
        }
        catch
        {
            return null;
        }
    }

    private AuthResult CreateSuccessResult()
    {
        return new AuthResult { Success = true };
    }

    private AuthResult CreateFailureResult(string? errorMessage)
    {
        return new AuthResult
        {
            Success = false,
            ErrorMessage = errorMessage ?? "Authentication failed"
        };
    }

    private string? ExtractTokenValue(JsonElement tokenData, string propertyName)
    {
        return tokenData.TryGetProperty(propertyName, out var value) ? value.GetString() : null;
    }

    private int ExtractExpiresIn(JsonElement tokenData)
    {
        return tokenData.TryGetProperty("expires_in", out var value) ? value.GetInt32() : 300;
    }

    private TokenResponse CreateTokenSuccessResponse(JsonElement tokenData)
    {
        return new TokenResponse
        {
            Success = true,
            AccessToken = ExtractTokenValue(tokenData, "access_token"),
            RefreshToken = ExtractTokenValue(tokenData, "refresh_token"),
            TokenType = ExtractTokenValue(tokenData, "token_type"),
            ExpiresIn = ExtractExpiresIn(tokenData)
        };
    }

    private TokenResponse CreateTokenFailureResponse()
    {
        return new TokenResponse
        {
            Success = false,
            ErrorMessage = "Invalid username or password"
        };
    }

    private class TokenResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
