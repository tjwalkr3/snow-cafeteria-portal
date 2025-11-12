using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;

public interface IBreakfastSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionDto> CurrentEntreeOptions { get; }
    List<FoodOptionTypeWithOptionsDto> CurrentOptionTypes { get; }

    string ActiveTab { get; }
    EntreeDto? SelectedEntree { get; }
    SideDto? SelectedSide { get; }
    DrinkDto? SelectedDrink { get; }

    Dictionary<int, string> SelectedOptionsByType { get; }

    int StationId { get; set; }
    int LocationId { get; set; }

    Task LoadDataAsync(int stationId, int locationId);
    void SetActiveTab(string tab);
    Task SelectEntree(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);
    void SetOptionForType(int optionTypeId, string optionName);
    string? GetSelectedOption(int optionTypeId);
    int GetSelectionCount();
    bool IsValidSelection();
    Task<bool> AddToOrderAsync();
}
