using Cafeteria.Api.Services.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cafeteria.Api.Authorization;

public class RequireUserRoleFilter : IAsyncActionFilter
{
    private readonly IUserRoleService _userRoleService;
    private readonly string[] _allowedRoles;

    public RequireUserRoleFilter(IUserRoleService userRoleService, string[] allowedRoles)
    {
        _userRoleService = userRoleService;
        _allowedRoles = allowedRoles;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var hasAllowedRole = await _userRoleService.UserHasAnyRoleAsync(user, _allowedRoles);
        if (!hasAllowedRole)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}