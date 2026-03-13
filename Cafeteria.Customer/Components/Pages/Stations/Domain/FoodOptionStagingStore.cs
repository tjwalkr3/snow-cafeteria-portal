using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class FoodOptionStagingStore
{
    public bool IsModalOpen { get; private set; }
    public string ModalTitle { get; private set; } = string.Empty;
    public EntreeDto? StagedEntree { get; private set; }
    public SideWithOptionsDto? StagedSide { get; private set; }
    public Dictionary<int, HashSet<string>> StagedSelections { get; private set; } = new();

    public void Open(EntreeDto entree, List<FoodOptionTypeWithOptionsDto> optionTypes, SelectionState state)
    {
        StagedEntree = entree;
        StagedSide = null;
        ModalTitle = entree.EntreeName;
        StagedSelections = new Dictionary<int, HashSet<string>>();

        foreach (var optionType in optionTypes)
        {
            var id = optionType.OptionType.Id;
            if (optionType.OptionType.MaxAmount > 1)
            {
                var existing = state.MultiSelectOptions.TryGetValue(id, out var list) ? list : new List<string>();
                StagedSelections[id] = new HashSet<string>(existing);
            }
            else
            {
                var existing = state.SingleSelectOptions.TryGetValue(id, out var val) ? val : null;
                StagedSelections[id] = existing != null ? new HashSet<string> { existing } : new HashSet<string>();
            }
        }

        IsModalOpen = true;
    }

    public void OpenForSide(SideWithOptionsDto side, SelectionState state)
    {
        StagedSide = side;
        StagedEntree = null;
        ModalTitle = side.Side.SideName;
        StagedSelections = new Dictionary<int, HashSet<string>>();

        foreach (var optionType in side.OptionTypes)
        {
            var id = optionType.OptionType.Id;
            var existing = state.SideOptions.TryGetValue(id, out var set) ? set : new HashSet<string>();
            StagedSelections[id] = new HashSet<string>(existing);
        }

        IsModalOpen = true;
    }

    public void Toggle(int optionTypeId, string name, FoodOptionTypeWithOptionsDto optionType, bool isCardOrder)
    {
        if (!StagedSelections.ContainsKey(optionTypeId))
            StagedSelections[optionTypeId] = new HashSet<string>();

        if (!StagedSelections[optionTypeId].Remove(name))
        {
            if (StagedSelections[optionTypeId].Count >= optionType.OptionType.MaxAmount)
                return;
            StagedSelections[optionTypeId].Add(name);
        }
    }

    public void Set(int optionTypeId, string name)
    {
        StagedSelections[optionTypeId] = new HashSet<string> { name };
    }

    public void Confirm(SelectionState state, List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        if (StagedSide != null)
        {
            state.SelectedSide = StagedSide.Side;
            foreach (var optionType in optionTypes)
            {
                var id = optionType.OptionType.Id;
                var staged = StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>();
                state.SideOptions[id] = staged;
            }
            StagedSide = null;
        }
        else
        {
            if (StagedEntree != null)
                state.SelectedEntree = StagedEntree;

            foreach (var optionType in optionTypes)
            {
                var id = optionType.OptionType.Id;
                var staged = StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>();

                if (optionType.OptionType.MaxAmount > 1)
                    state.MultiSelectOptions[id] = staged.ToList();
                else
                {
                    var selected = staged.FirstOrDefault();
                    if (selected != null)
                        state.SingleSelectOptions[id] = selected;
                }
            }
        }

        IsModalOpen = false;
    }

    public void Discard()
    {
        StagedEntree = null;
        StagedSide = null;
        StagedSelections.Clear();
        IsModalOpen = false;
    }
}
