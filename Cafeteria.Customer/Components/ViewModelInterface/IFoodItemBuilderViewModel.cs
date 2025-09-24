using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IFoodItemBuilderViewModel
{
    FoodItemDto SelectedFoodItem { get; set; }
    List<IngredientTypeDto> AvailableIngredientTypes { get; set; }
    List<IngredientDto> AvailableIngredients { get; set; }
    List<IngredientDto> SelectedIngredients { get; set; }
    Dictionary<IngredientTypeDto, List<IngredientDto>> IngredientsByType { get; set; }
    void SelectIngredient(IngredientDto ingredient);
    void UnselectIngredient(IngredientDto ingredient);
}