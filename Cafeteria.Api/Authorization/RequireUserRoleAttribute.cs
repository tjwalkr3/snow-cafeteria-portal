using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireUserRoleAttribute : TypeFilterAttribute
{
    public RequireUserRoleAttribute(params string[] allowedRoles)
        : base(typeof(RequireUserRoleFilter))
    {
        Arguments = new object[] { allowedRoles };
    }
}