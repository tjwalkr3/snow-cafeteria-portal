using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Drink;

public class CreateOrEditDrinkVM : ICreateOrEditDrinkVM
{
    private readonly IDrinkService _drinkService;
    private readonly IDrinkVM _parentVM;

    public DrinkDto CurrentDrink { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }

    public CreateOrEditDrinkVM(IDrinkService drinkService, IDrinkVM parentVM)
    {
        _drinkService = drinkService;
        _parentVM = parentVM;
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
