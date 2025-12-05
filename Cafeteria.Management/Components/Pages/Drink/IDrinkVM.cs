using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Drink;

public interface IDrinkVM
{
    List<DrinkDto> Drinks { get; set; }
    Task LoadDrinks();
    Task DeleteDrink(int id);
    Task ShowCreateModal();
    Task ShowEditModal(int id);
    void HideModal();
}
