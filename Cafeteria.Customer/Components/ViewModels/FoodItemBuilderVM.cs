using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;

namespace Cafeteria.Customer.Components.ViewModels;
public class FoodItemBuilderVM : IFoodItemBuilderViewModel
{
    public FoodItemBuilderVM()
    {
        SelectedFoodItem = DummyData.CreateTurkeysandwich();
        AvailableIngredients = DummyData.GetIngredientList;
        IngredientsByType = GetIngredientsByType();

    }

    public FoodItemBuilderVM(FoodItemDto selectedItem)
    {
        SelectedFoodItem = selectedItem;
    }
    public FoodItemDto SelectedFoodItem { get; set; }
    public List<IngredientTypeDto> AvailableIngredientTypes { get; set; } = new List<IngredientTypeDto>();
    public List<IngredientDto> AvailableIngredients { get; set; } = new List<IngredientDto>();
    public List<IngredientDto> SelectedIngredients { get; set; } = new List<IngredientDto>();
    public Dictionary<IngredientTypeDto, List<IngredientDto>> IngredientsByType { get; set; } = new Dictionary<IngredientTypeDto, List<IngredientDto>>();

    public void SelectIngredient(IngredientDto ingredient)
    {
        if (!SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Add(ingredient);
        }
    }

    public void UnselectIngredient(IngredientDto ingredient)
    {
        if (SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Remove(ingredient);
        }
    }

    private Dictionary<IngredientTypeDto, List<IngredientDto>> GetIngredientsByType()
    {
        Dictionary<IngredientTypeDto, List<IngredientDto>> ingredients = new();

        AvailableIngredientTypes = DummyData.GetIngredientTypeList;
        var ingredientTypeMappings = DummyData.GetIngredientIngredientTypeList;

        foreach (var type in AvailableIngredientTypes)
        {
            var ingredientIdsForType = ingredientTypeMappings
                .Where(mapping => mapping.IngredientTypeId == type.Id)
                .Select(mapping => mapping.IngredientId)
                .ToList();

            ingredients[type] = AvailableIngredients
                .Where(ingredient => ingredientIdsForType.Contains(ingredient.Id))
                .ToList();
        }

        return ingredients;
    }
}