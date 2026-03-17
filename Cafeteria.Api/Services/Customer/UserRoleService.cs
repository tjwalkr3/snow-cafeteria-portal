using System.Data;
using System.Security.Claims;
using Dapper;

namespace Cafeteria.Api.Services.Customer;

public class UserRoleService : IUserRoleService
{
    private readonly IDbConnection _dbConnection;

    public UserRoleService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string?> GetUserRoleAsync(ClaimsPrincipal user)
    {
        var email = user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }

        const string sql = @"
            SELECT user_role
            FROM cafeteria.customer
            WHERE email = @Email
            LIMIT 1";

        var role = await _dbConnection.QuerySingleOrDefaultAsync<string?>(sql, new { Email = email });

        return string.IsNullOrWhiteSpace(role) ? null : role;
    }

    public async Task<bool> UserHasAnyRoleAsync(ClaimsPrincipal user, params string[] allowedRoles)
    {
        if (allowedRoles == null || allowedRoles.Length == 0)
        {
            return false;
        }

        var role = await GetUserRoleAsync(user);
        if (role == null)
        {
            return false;
        }

        return allowedRoles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
    }
}