using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Drink;

public class CreateOrEditDrinkVM : ICreateOrEditDrinkVM
{
    private readonly IDrinkService _drinkService;
    private readonly IDrinkVM _parentVM;
    private readonly IStationService _stationService;

    public DrinkDto CurrentDrink { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public List<StationDto> Stations { get; set; } = [];
    public string? SelectedStationName { get; set; }

    public CreateOrEditDrinkVM(IDrinkService drinkService, IDrinkVM parentVM, IStationService stationService)
    {
        _drinkService = drinkService;
        _parentVM = parentVM;
        _stationService = stationService;
    }

    public async Task LoadStations()
    {
        try
        {
            Stations = await _stationService.GetAllStations();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading stations: {ex.Message}");
        }
    }

    public void SetSelectedStation(int stationId)
    {
        CurrentDrink.StationId = stationId;
        SelectedStationName = Stations.FirstOrDefault(s => s.Id == stationId)?.StationName ?? "Unknown";
    }

    public async Task SaveDrink()
    {
        try
        {
            if (IsEditing)
            {
                await _drinkService.UpdateDrinkById(CurrentDrink.Id, CurrentDrink);
            }
            else
            {
                await _drinkService.CreateDrink(CurrentDrink);
            }

            IsVisible = false;
            await _parentVM.LoadDrinks();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving drink: {ex.Message}");
            throw;
        }
    }
}
