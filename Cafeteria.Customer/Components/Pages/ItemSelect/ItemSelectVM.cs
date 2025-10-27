using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public class ItemSelectVM : IItemSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    public StationDtoOld? SelectedStation { get; private set; }

    public ItemSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<List<FoodItemDtoOld>> GetFoodItemsAsync()
    {
        if (SelectedStation != null && !ErrorOccurredWhileParsingSelectedStation())
        {
            return await _menuService.GetFoodItemsByStation(SelectedStation.Id);
        }
        return new List<FoodItemDtoOld>(); // Return empty list when no station selected
    }

    public async Task GetDataFromRouteParameters(string uri)
    {
        await Task.Delay(0); // Simulate async work

        string queryString = uri.Substring(uri.IndexOf('?') + 1);
        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
        try
        {
            StationDtoOld station = JsonSerializer.Deserialize<StationDtoOld>(queryParams.Get("station") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize station from query parameter.");
            SelectedStation = station;
        }
        catch
        {
            urlParsingFailed = true;
        }
    }

    public bool ErrorOccurredWhileParsingSelectedStation()
    {
        return urlParsingFailed;
    }
}