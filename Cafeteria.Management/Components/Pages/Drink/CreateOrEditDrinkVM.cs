using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.Drinks;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Drink;

public class CreateOrEditDrinkVM : ICreateOrEditDrinkVM
{
    private readonly IDrinkService _drinkService;
    private readonly IDrinkVM _parentVM;
    private readonly ILocationService _locationService;

    public DrinkDto CurrentDrink { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }
    public List<LocationDto> Locations { get; set; } = [];

    public CreateOrEditDrinkVM(IDrinkService drinkService, IDrinkVM parentVM, ILocationService locationService)
    {
        _drinkService = drinkService;
        _parentVM = parentVM;
        _locationService = locationService;
    }

    public async Task LoadLocations()
    {
        try
        {
            Locations = await _locationService.GetAllLocations();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading locations: {ex.Message}");
        }
    }

    public bool ValidateDrink(IEnumerable<DrinkDto> existingDrinks, DrinkDto newDrink)
    {
        return !existingDrinks.Any(d =>
            d.DrinkName.Equals(newDrink.DrinkName, StringComparison.OrdinalIgnoreCase) &&
            d.LocationId == newDrink.LocationId &&
            d.Id != newDrink.Id);
    }

    public async Task<bool> SaveDrink()
    {
        if (!ValidateDrink(_parentVM.Drinks, CurrentDrink))
        {
            ShowToast = true;
            ToastMessage = "A drink with this name already exists in this location.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

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
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving drink: {ex.Message}");
            throw;
        }
    }
}
