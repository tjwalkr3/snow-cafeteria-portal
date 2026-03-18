using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class SelectionState
{
    public EntreeDto? SelectedEntree { get; set; }
    public SideDto? SelectedSide { get; set; }
    public DrinkDto? SelectedDrink { get; set; }

    // For single-select options (e.g., Breakfast - one option per type)
    public Dictionary<int, string> SingleSelectOptions { get; } = new();

    public Dictionary<int, List<string>> MultiSelectOptions { get; } = new();

    public Dictionary<int, HashSet<string>> SideOptions { get; } = new();

    public void Clear()
    {
        SelectedEntree = null;
        SelectedSide = null;
        SelectedDrink = null;
        SingleSelectOptions.Clear();
        MultiSelectOptions.Clear();
        SideOptions.Clear();
    }

    public void ClearOptionsOnly()
    {
        SingleSelectOptions.Clear();
        MultiSelectOptions.Clear();
    }

    public bool HasAnySelection()
    {
        return SelectedEntree != null || SelectedSide != null || SelectedDrink != null;
    }

    public int GetTotalSelectionCount()
    {
        int count = 0;
        if (SelectedEntree != null) count++;
        if (SelectedSide != null) count++;
        if (SelectedDrink != null) count++;
        return count;
    }
}
