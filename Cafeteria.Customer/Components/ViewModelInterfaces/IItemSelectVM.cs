using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IItemSelectVM
{
    StationDto? SelectedStation { get; }
    List<FoodItemDto> GetFoodItems();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedStation();
}