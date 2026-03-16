using System.Security.Claims;

namespace Cafeteria.Api.Services.Customer;

public interface IUserRoleService
{
    Task<string?> GetUserRoleAsync(ClaimsPrincipal user);
    Task<bool> UserHasAnyRoleAsync(ClaimsPrincipal user, params string[] allowedRoles);
}