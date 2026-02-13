using Cafeteria.Shared.Services.Auth;
using Cafeteria.Management.Services.Customers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Management.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("signin")]
    public async Task<IActionResult> SignInCustomer(
        [FromQuery] string token,
        [FromQuery] string? returnUrl,
        [FromServices] ICustomerRegistrationService customerRegistrationService)
    {
        try
        {
            var authData = AuthService.ParseTokenAndCreateAuthData(token, DateTimeOffset.UtcNow);

            if (authData == null)
            {
                return Redirect("/signin?error=invalid_session");
            }

            var (claimsPrincipal, authProperties) = authData.Value;

            var accessToken = authProperties.GetTokenValue("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                return Redirect("/signin?error=invalid_session");
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            try
            {
                await customerRegistrationService.RegisterOrUpdateCustomerAsync(accessToken);
            }
            catch
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/signin?error=registration_failed");
            }

            return Redirect(returnUrl ?? "/");
        }
        catch
        {
            return Redirect("/signin?error=invalid_session");
        }
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOutCustomer()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
