using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Cafeteria.Customer.Services.Cart;

public class CartKeyService : ICartKeyService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CartKeyService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string> GetCartKey()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.FindFirst(ClaimTypes.Email)?.Value
            ?? authState.User.FindFirst("email")?.Value
            ?? "order";
    }
}
