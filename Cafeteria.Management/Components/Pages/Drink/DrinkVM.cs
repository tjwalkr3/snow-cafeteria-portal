using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Drink;

public class DrinkVM : IDrinkVM
{
    private readonly IDrinkService _drinkService;
    public ICreateOrEditDrinkVM? CreateOrEditVM { get; set; }

    public List<DrinkDto> Drinks { get; set; } = [];

    public DrinkVM(IDrinkService drinkService)
    {
        _drinkService = drinkService;
    }

    public void SetCreateOrEditVM(ICreateOrEditDrinkVM vm)
    {
        CreateOrEditVM = vm;
    }

    public async Task LoadDrinks()
    {
        try
        {
            Drinks = await _drinkService.GetAllDrinks();
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"Error loading drinks: {ex.Message}");
        }
    }

    public async Task DeleteDrink(int id)
    {
        try
        {
            await _drinkService.DeleteDrinkById(id);
            await LoadDrinks();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting drink: {ex.Message}");
            throw;
        }
    }

    public Task ShowCreateModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.CurrentDrink = new DrinkDto();
            CreateOrEditVM.IsEditing = false;
            CreateOrEditVM.IsVisible = true;
        }
        return Task.CompletedTask;
    }

    public async Task ShowEditModal(int id)
    {
        if (CreateOrEditVM != null)
        {
            var drink = await _drinkService.GetDrinkById(id);
            if (drink != null)
            {
                CreateOrEditVM.CurrentDrink = drink;
                CreateOrEditVM.IsEditing = true;
                CreateOrEditVM.IsVisible = true;
            }
        }
    }

    public async Task ToggleStockStatus(int id, bool inStock)
    {
        try
        {
            await _drinkService.SetStockStatusById(id, inStock);
            var drink = Drinks.FirstOrDefault(d => d.Id == id);
            if (drink != null)
            {
                drink.InStock = inStock;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling stock status: {ex.Message}");
        }
    }

    public void HideModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.IsVisible = false;
        }
    }
}
