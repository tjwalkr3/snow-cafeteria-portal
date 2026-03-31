using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public sealed class CardOrderDraftManager
{
    private readonly Dictionary<int, int> _entreeQtys = new();
    private readonly Dictionary<int, int> _sideQtys = new();
    private readonly Dictionary<int, int> _drinkQtys = new();

    private readonly Dictionary<int, Dictionary<int, HashSet<string>>> _entreeOptions = new();
    private readonly Dictionary<int, List<FoodOptionTypeWithOptionsDto>> _entreeOptionTypes = new();

    private readonly Dictionary<int, Dictionary<int, HashSet<string>>> _sideOptions = new();
    private readonly Dictionary<int, List<FoodOptionTypeWithOptionsDto>> _sideOptionTypes = new();

    public IReadOnlyDictionary<int, int> EntreeQuantities => _entreeQtys;

    public IReadOnlyDictionary<int, int> SideQuantities => _sideQtys;

    public IReadOnlyDictionary<int, int> DrinkQuantities => _drinkQtys;

    public bool HasAnySelection() => _entreeQtys.Any() || _sideQtys.Any() || _drinkQtys.Any();

    public bool HasEntreeSelection() => _entreeQtys.Any();

    public bool HasSideSelection() => _sideQtys.Any();

    public bool HasDrinkSelection() => _drinkQtys.Any();

    public int TotalEntreeCount() => _entreeQtys.Values.Sum();

    public int TotalSideCount() => _sideQtys.Values.Sum();

    public int TotalDrinkCount() => _drinkQtys.Values.Sum();

    public bool ContainsEntree(int entreeId) => _entreeQtys.ContainsKey(entreeId);

    public bool ContainsSide(int sideId) => _sideQtys.ContainsKey(sideId);

    public void IncrementEntree(int entreeId)
    {
        _entreeQtys[entreeId] = _entreeQtys.GetValueOrDefault(entreeId, 0) + 1;
    }

    public void IncrementSide(int sideId)
    {
        _sideQtys[sideId] = _sideQtys.GetValueOrDefault(sideId, 0) + 1;
    }

    public void IncrementDrink(int drinkId)
    {
        _drinkQtys[drinkId] = _drinkQtys.GetValueOrDefault(drinkId, 0) + 1;
    }

    public void SetEntreeSelection(
        int entreeId,
        IReadOnlyList<FoodOptionTypeWithOptionsDto> optionTypes,
        IReadOnlyDictionary<int, HashSet<string>> selectionsByType)
    {
        _entreeOptionTypes[entreeId] = [.. optionTypes];
        _entreeOptions[entreeId] = CloneSelections(selectionsByType);
        if (!_entreeQtys.ContainsKey(entreeId))
            _entreeQtys[entreeId] = 1;
    }

    public void SetSideSelection(
        int sideId,
        IReadOnlyList<FoodOptionTypeWithOptionsDto> optionTypes,
        IReadOnlyDictionary<int, HashSet<string>> selectionsByType)
    {
        _sideOptionTypes[sideId] = [.. optionTypes];
        _sideOptions[sideId] = CloneSelections(selectionsByType);
        if (!_sideQtys.ContainsKey(sideId))
            _sideQtys[sideId] = 1;
    }

    public SelectionState CreateSideTempSelectionState(int sideId)
    {
        var state = new SelectionState();
        if (_sideOptions.TryGetValue(sideId, out var existing))
        {
            foreach (var (optionTypeId, selected) in existing)
            {
                state.SideOptions[optionTypeId] = new HashSet<string>(selected);
            }
        }

        return state;
    }

    public void SetEntreeQuantity(int entreeId, int quantity)
    {
        if (quantity <= 0)
        {
            _entreeQtys.Remove(entreeId);
            _entreeOptions.Remove(entreeId);
            _entreeOptionTypes.Remove(entreeId);
            return;
        }

        _entreeQtys[entreeId] = quantity;
    }

    public void SetSideQuantity(int sideId, int quantity)
    {
        if (quantity <= 0)
        {
            _sideQtys.Remove(sideId);
            _sideOptions.Remove(sideId);
            _sideOptionTypes.Remove(sideId);
            return;
        }

        _sideQtys[sideId] = quantity;
    }

    public void SetDrinkQuantity(int drinkId, int quantity)
    {
        if (quantity <= 0)
        {
            _drinkQtys.Remove(drinkId);
            return;
        }

        _drinkQtys[drinkId] = quantity;
    }

    public CardStationDraft CreateSnapshot(
        IReadOnlyList<EntreeDto> entrees,
        IReadOnlyList<SideWithOptionsDto> sides,
        IReadOnlyList<DrinkDto> drinks)
    {
        return new CardStationDraft
        {
            Entrees = entrees,
            Sides = sides,
            Drinks = drinks,
            EntreeQuantities = new Dictionary<int, int>(_entreeQtys),
            SideQuantities = new Dictionary<int, int>(_sideQtys),
            DrinkQuantities = new Dictionary<int, int>(_drinkQtys),
            EntreeOptionTypes = _entreeOptionTypes.ToDictionary(
                kv => kv.Key,
                kv => new List<FoodOptionTypeWithOptionsDto>(kv.Value)),
            SideOptionTypes = _sideOptionTypes.ToDictionary(
                kv => kv.Key,
                kv => new List<FoodOptionTypeWithOptionsDto>(kv.Value)),
            EntreeOptions = _entreeOptions.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToDictionary(inner => inner.Key, inner => new HashSet<string>(inner.Value))),
            SideOptions = _sideOptions.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToDictionary(inner => inner.Key, inner => new HashSet<string>(inner.Value)))
        };
    }

    private static Dictionary<int, HashSet<string>> CloneSelections(IReadOnlyDictionary<int, HashSet<string>> selectionsByType)
    {
        return selectionsByType.ToDictionary(
            kv => kv.Key,
            kv => new HashSet<string>(kv.Value));
    }
}