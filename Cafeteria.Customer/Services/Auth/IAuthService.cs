namespace Cafeteria.Customer.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> ValidateCredentialsAsync(string username, string password);
}
