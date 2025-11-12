using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;

public interface IDeliSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }
    List<FoodOptionDto> AllEntreeOptions { get; }
    List<FoodOptionTypeWithOptionsDto> OptionTypes { get; }

    string ActiveTab { get; }
    SideDto? SelectedSide { get; }
    DrinkDto? SelectedDrink { get; }

    Dictionary<int, List<string>> SelectedOptionsByType { get; }

    string? SelectedBread { get; }
    string? SelectedMeat { get; }
    string? SelectedCheese { get; }
    List<string> SelectedToppings { get; }
    string? SelectedDressing { get; }

    List<string> BreadOptions { get; }
    List<string> MeatOptions { get; }
    List<string> CheeseOptions { get; }
    List<string> ToppingOptions { get; }
    List<string> DressingOptions { get; }

    int StationId { get; set; }
    int LocationId { get; set; }

    Task LoadDataAsync(int stationId, int locationId);
    void SetActiveTab(string tab);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);
    void ToggleTopping(string topping);
    void SetBread(string bread);
    void SetMeat(string meat);
    void SetCheese(string cheese);
    void SetDressing(string dressing);

    void ToggleOptionForType(int optionTypeId, string optionName);
    void SetOptionForType(int optionTypeId, string optionName);
    List<string> GetSelectedOptionsForType(int optionTypeId);
    bool IsOptionSelected(int optionTypeId, string optionName);

    int GetSelectionCount();
    string GetSelectionSummary();
    bool IsValidSelection();
    Task<bool> AddToOrderAsync();
}
