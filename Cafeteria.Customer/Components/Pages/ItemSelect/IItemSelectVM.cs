using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public interface IItemSelectVM
{
    StationDto? SelectedStation { get; }
    LocationDto? SelectedLocation { get; }
    Task<List<FoodItemDtoOld>> GetFoodItemsAsync();
    Task<List<EntreeDto>> GetEntreesAsync();
    Task<List<SideDto>> GetSidesAsync();
    Task<List<DrinkDto>> GetDrinksAsync();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingUrlParameters();
}