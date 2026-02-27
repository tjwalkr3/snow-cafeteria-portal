using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class FoodOptionStagingStore
{
    public bool IsModalOpen { get; private set; }
    public string ModalTitle { get; private set; } = string.Empty;
    public EntreeDto? StagedEntree { get; private set; }
    public Dictionary<int, HashSet<string>> StagedSelections { get; private set; } = new();

    public void Open(EntreeDto entree, List<FoodOptionTypeWithOptionsDto> optionTypes, SelectionState state)
    {
        StagedEntree = entree;
        ModalTitle = entree.EntreeName;
        StagedSelections = new Dictionary<int, HashSet<string>>();

        foreach (var optionType in optionTypes)
        {
            var id = optionType.OptionType.Id;
            if (OptionTypeHelper.IsMultiSelectOptionType(optionType))
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

    public void Toggle(int optionTypeId, string name, FoodOptionTypeWithOptionsDto optionType, bool isCardOrder)
    {
        if (!StagedSelections.ContainsKey(optionTypeId))
            StagedSelections[optionTypeId] = new HashSet<string>();

        if (!StagedSelections[optionTypeId].Remove(name))
        {
            if (!isCardOrder && StagedSelections[optionTypeId].Count >= optionType.OptionType.NumIncluded)
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
        if (StagedEntree != null)
            state.SelectedEntree = StagedEntree;

        foreach (var optionType in optionTypes)
        {
            var id = optionType.OptionType.Id;
            var staged = StagedSelections.GetValueOrDefault(id) ?? new HashSet<string>();

            if (OptionTypeHelper.IsMultiSelectOptionType(optionType))
                state.MultiSelectOptions[id] = staged.ToList();
            else
            {
                var selected = staged.FirstOrDefault();
                if (selected != null)
                    state.SingleSelectOptions[id] = selected;
            }
        }

        IsModalOpen = false;
    }

    public void Discard()
    {
        StagedEntree = null;
        StagedSelections.Clear();
        IsModalOpen = false;
    }
}
