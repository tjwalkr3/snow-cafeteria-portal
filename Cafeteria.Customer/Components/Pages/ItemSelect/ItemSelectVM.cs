using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public class ItemSelectVM : IItemSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    public StationDto? SelectedStation { get; private set; }
    public LocationDto? SelectedLocation { get; private set; }

    public ItemSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<List<FoodItemDtoOld>> GetFoodItemsAsync() // This method to be deleted when UI is updated
    { 
        if (SelectedStation != null && !ErrorOccurredWhileParsingUrlParameters())
        {
            return await _menuService.GetFoodItemsByStation(SelectedStation.Id);
        }
        return new List<FoodItemDtoOld>(); // Return empty list when no station selected
    }

    public async Task<List<EntreeDto>> GetEntreesAsync()
    {
        if (SelectedStation != null && !ErrorOccurredWhileParsingUrlParameters())
        {
            return await _menuService.GetEntreesByStation(SelectedStation.Id);
        }
        return new List<EntreeDto>(); // Return empty list when no station selected
    }

    public async Task<List<SideDto>> GetSidesAsync()
    {
        if (SelectedStation != null && !ErrorOccurredWhileParsingUrlParameters())
        {
            return await _menuService.GetSidesByStation(SelectedStation.Id);
        }
        return new List<SideDto>(); // Return empty list when no station selected
    }

    public async Task<List<DrinkDto>> GetDrinksAsync()
    {
        if (SelectedStation != null && !ErrorOccurredWhileParsingUrlParameters())
        {
            return await _menuService.GetDrinksByLocation(SelectedLocation.Id);
        }
        return new List<DrinkDto>(); // Return empty list when no station selected
    }

    public async Task GetDataFromRouteParameters(string uri)
    {
        await Task.Delay(0); // Simulate async work

        string queryString = uri.Substring(uri.IndexOf('?') + 1);
        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
        try
        {
            StationDto station = JsonSerializer.Deserialize<StationDto>(queryParams.Get("station") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize station from query parameter.");
            SelectedStation = station;
            LocationDto location = JsonSerializer.Deserialize<LocationDto>(queryParams.Get("location") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize location from query parameter.");
            SelectedLocation = location;
        }
        catch
        {
            urlParsingFailed = true;
        }
    }

    public bool ErrorOccurredWhileParsingUrlParameters()
    {
        return urlParsingFailed;
    }
}