namespace Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Shared.DTOs;
using System.Text.Json;

public class ItemSelectVM : IItemSelectVM
{
    string errorString = "Error";
    public StationDto? SelectedStation { get; private set; }

    public List<FoodItemDto> GetFoodItems()
    {
        if (SelectedStation != null && !ErrorOccurredWhileParsingSelectedStation())
        {
            return DummyData.GetFoodItemsByStation(SelectedStation.Id);
        }
        return DummyData.GetFoodItemList;
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
        }
        catch
        {
            SelectedStation = new();
            SelectedStation.StationName = errorString;
        }
    }

    public bool ErrorOccurredWhileParsingSelectedStation()
    {
        return SelectedStation != null && SelectedStation.StationName == errorString;
    }
}