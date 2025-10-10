using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public interface IItemSelectVM
{
    StationDto? SelectedStation { get; }
    Task<List<FoodItemDto>> GetFoodItemsAsync();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedStation();
}