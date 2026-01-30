using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.Api.Services.Swipes;

public interface ISwipeService
{
    Task<SwipeDto> GetSwipesByUserID(int userId);
}