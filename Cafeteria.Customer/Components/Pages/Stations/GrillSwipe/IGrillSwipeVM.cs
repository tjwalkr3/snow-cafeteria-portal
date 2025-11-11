using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.GrillSwipe;

public interface IGrillSwipeVM
{
    List<EntreeDto> Entrees { get; }
    List<SideDto> Sides { get; }
    List<DrinkDto> Drinks { get; }

    string ActiveTab { get; }
    EntreeDto? SelectedEntree { get; }
    SideDto? SelectedSide { get; }
    DrinkDto? SelectedDrink { get; }

    int StationId { get; set; }
    int LocationId { get; set; }

    Task LoadDataAsync(int stationId, int locationId);
    void SetActiveTab(string tab);
    void SelectEntree(EntreeDto entree);
    void SelectSide(SideDto side);
    void SelectDrink(DrinkDto drink);

    int GetSelectionCount();
    bool IsValidSelection();
    Task<bool> AddToOrderAsync();
}
