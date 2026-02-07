namespace Cafeteria.Customer.Services.Auth;

public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SessionToken { get; set; }
}
