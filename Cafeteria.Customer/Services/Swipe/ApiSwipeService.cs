using Cafeteria.Customer.Services.Auth;
using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.Customer.Services.Swipe;

public class ApiSwipeService(IHttpClientAuth client) : IApiSwipeService
{
    public async Task<SwipeDto?> GetSwipesByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        return await client.GetAsync<SwipeDto>($"swipe/email/{email}");
    }
}
