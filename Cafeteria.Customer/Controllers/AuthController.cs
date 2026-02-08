using Cafeteria.Customer.Services.Auth;
using Cafeteria.Customer.Services.Customer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Customer.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("signin")]
    public async Task<IActionResult> SignInCustomer(
        [FromQuery] string token,
        [FromQuery] string? returnUrl,
        [FromServices] ICustomerService customerService)
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
                await customerService.RegisterOrUpdateCustomerAsync(accessToken);
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
