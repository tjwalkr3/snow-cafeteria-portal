using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IItemSelectVM
{
    StationDto? SelectedStation { get; }
    Task<List<FoodItemDto>> GetFoodItemsAsync();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedStation();
}