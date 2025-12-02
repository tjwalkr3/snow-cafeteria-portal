using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Drink;

public interface ICreateOrEditDrinkVM
{
    DrinkDto CurrentDrink { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    Task SaveDrink();
}
