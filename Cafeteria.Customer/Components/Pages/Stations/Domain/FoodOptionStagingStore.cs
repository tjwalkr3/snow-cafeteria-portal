using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public class FoodOptionStagingStore
{
    // Builder modal (all option types at once)
    public bool IsBuilderOpen { get; private set; }
    public string BuilderTitle { get; private set; } = string.Empty;
    public Dictionary<int, HashSet<string>> StagedBuilderSelections { get; private set; } = new();

    // Single-type modal (one option type drill-down)
    public bool IsSingleTypeOpen { get; private set; }
    public int ActiveOptionTypeId { get; private set; }
    public HashSet<string> StagedSingleTypeSelections { get; private set; } = new();

    // Options modal (single-select per type, e.g. breakfast)
    public bool IsOptionsOpen { get; private set; }
    public EntreeDto? StagedEntree { get; private set; }
    public Dictionary<int, string> StagedSingleSelectOptions { get; private set; } = new();

    // Toppings modal (pizza)
    public bool IsToppingsOpen { get; private set; }
    public HashSet<string> StagedToppings { get; private set; } = new();

    // Builder modal lifecycle
    public void OpenBuilder(string title, List<FoodOptionTypeWithOptionsDto> optionTypes, SelectionState state)
    {
        BuilderTitle = title;
        StagedBuilderSelections = new Dictionary<int, HashSet<string>>();
        foreach (var optionType in optionTypes)
        {
            var id = optionType.OptionType.Id;
            var existing = state.MultiSelectOptions.TryGetValue(id, out var list) ? list : new List<string>();
            StagedBuilderSelections[id] = new HashSet<string>(existing);
        }
        IsBuilderOpen = true;
    }

    public void CloseBuilder() => IsBuilderOpen = false;

    public void ToggleStagedBuilderOption(int optionTypeId, string name, List<FoodOptionTypeWithOptionsDto> optionTypes, bool isCardOrder)
    {
        if (!StagedBuilderSelections.ContainsKey(optionTypeId))
            StagedBuilderSelections[optionTypeId] = new HashSet<string>();

        if (!StagedBuilderSelections[optionTypeId].Remove(name))
        {
            if (!isCardOrder)
            {
                var optionType = optionTypes.FirstOrDefault(o => o.OptionType.Id == optionTypeId);
                if (optionType != null && StagedBuilderSelections[optionTypeId].Count >= optionType.OptionType.NumIncluded)
                    return;
            }
            StagedBuilderSelections[optionTypeId].Add(name);
        }
    }

    public void SetStagedBuilderOption(int optionTypeId, string name)
    {
        StagedBuilderSelections[optionTypeId] = new HashSet<string> { name };
    }

    public void ConfirmBuilder(SelectionState state, List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        foreach (var optionType in optionTypes)
        {
            var id = optionType.OptionType.Id;
            var staged = StagedBuilderSelections.GetValueOrDefault(id) ?? new HashSet<string>();

            if (OptionTypeHelper.IsMultiSelectOptionType(optionType))
                state.MultiSelectOptions[id] = staged.ToList();
            else
            {
                var selected = staged.FirstOrDefault();
                if (selected != null)
                    state.SingleSelectOptions[id] = selected;
            }
        }
        IsBuilderOpen = false;
    }

    // Single-type modal lifecycle
    public void OpenSingleType(int optionTypeId, SelectionState state)
    {
        ActiveOptionTypeId = optionTypeId;
        var existing = state.MultiSelectOptions.TryGetValue(optionTypeId, out var list) ? list : new List<string>();
        StagedSingleTypeSelections = new HashSet<string>(existing);
        IsSingleTypeOpen = true;
    }

    public void CloseSingleType() => IsSingleTypeOpen = false;

    public void ToggleStagedSingleTypeOption(string name, FoodOptionTypeWithOptionsDto optionType, bool isCardOrder)
    {
        if (!StagedSingleTypeSelections.Remove(name))
        {
            if (!isCardOrder && StagedSingleTypeSelections.Count >= optionType.OptionType.NumIncluded)
                return;
            StagedSingleTypeSelections.Add(name);
        }
    }

    public void SetStagedSingleTypeOption(string name)
    {
        StagedSingleTypeSelections.Clear();
        StagedSingleTypeSelections.Add(name);
    }

    public void ConfirmSingleType(SelectionState state, List<FoodOptionTypeWithOptionsDto> optionTypes)
    {
        var activeOptionType = optionTypes.FirstOrDefault(o => o.OptionType.Id == ActiveOptionTypeId);
        var isMulti = activeOptionType != null && OptionTypeHelper.IsMultiSelectOptionType(activeOptionType);

        if (isMulti)
            state.MultiSelectOptions[ActiveOptionTypeId] = StagedSingleTypeSelections.ToList();
        else
        {
            var selected = StagedSingleTypeSelections.FirstOrDefault();
            if (selected != null)
                state.SingleSelectOptions[ActiveOptionTypeId] = selected;
        }
        IsSingleTypeOpen = false;
    }

    // Options modal lifecycle (single-select per type, e.g. breakfast)
    public void OpenOptions(EntreeDto entree, SelectionState state)
    {
        StagedEntree = entree;
        StagedSingleSelectOptions = new Dictionary<int, string>(state.SingleSelectOptions);
        state.SelectedEntree = null;
        state.SingleSelectOptions.Clear();
        IsOptionsOpen = true;
    }

    public void CloseOptions()
    {
        StagedEntree = null;
        StagedSingleSelectOptions.Clear();
        IsOptionsOpen = false;
    }

    public void SetStagedSingleSelectOption(int optionTypeId, string optionName)
    {
        StagedSingleSelectOptions[optionTypeId] = optionName;
    }

    public void ConfirmOptions(SelectionState state)
    {
        if (StagedEntree != null)
            state.SelectedEntree = StagedEntree;

        foreach (var kvp in StagedSingleSelectOptions)
            state.SingleSelectOptions[kvp.Key] = kvp.Value;

        StagedEntree = null;
        IsOptionsOpen = false;
    }

    // Toppings modal lifecycle (pizza)
    public void OpenToppings(EntreeDto entree, SelectionState state)
    {
        StagedEntree = entree;
        StagedToppings = new HashSet<string>(state.SelectedToppings);
        state.SelectedEntree = null;
        state.SelectedToppings.Clear();
        IsToppingsOpen = true;
    }

    public void ReopenToppings(SelectionState state)
    {
        StagedToppings = new HashSet<string>(state.SelectedToppings);
        IsToppingsOpen = true;
    }

    public void CloseToppings()
    {
        StagedEntree = null;
        StagedToppings.Clear();
        IsToppingsOpen = false;
    }

    public void ToggleStagedTopping(string topping, List<FoodOptionTypeWithOptionsDto> optionTypes, bool isCardOrder)
    {
        if (!StagedToppings.Remove(topping))
        {
            if (!isCardOrder)
            {
                var toppingsOptionType = optionTypes.FirstOrDefault();
                if (toppingsOptionType != null && StagedToppings.Count >= toppingsOptionType.OptionType.NumIncluded)
                    return;
            }
            StagedToppings.Add(topping);
        }
    }

    public void ConfirmToppings(SelectionState state)
    {
        if (StagedEntree != null)
            state.SelectedEntree = StagedEntree;

        state.SelectedToppings.Clear();
        state.SelectedToppings.AddRange(StagedToppings);

        StagedEntree = null;
        IsToppingsOpen = false;
    }
}
