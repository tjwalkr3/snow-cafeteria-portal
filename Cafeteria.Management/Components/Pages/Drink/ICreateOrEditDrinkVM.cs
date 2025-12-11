using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Drink;

public interface ICreateOrEditDrinkVM
{
    DrinkDto CurrentDrink { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }
    List<StationDto> Stations { get; set; }
    Task<bool> SaveDrink();
    Task LoadStations();
}
