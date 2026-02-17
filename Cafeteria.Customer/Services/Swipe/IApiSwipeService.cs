using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.Customer.Services.Swipe;

public interface IApiSwipeService
{
    Task<SwipeDto?> GetSwipesByEmail(string email);
}
