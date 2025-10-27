using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public interface IItemSelectVM
{
    StationDtoOld? SelectedStation { get; }
    Task<List<FoodItemDtoOld>> GetFoodItemsAsync();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedStation();
}