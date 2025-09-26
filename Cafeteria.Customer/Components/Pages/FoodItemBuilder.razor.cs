using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages;

public partial class FoodItemBuilder
{
    protected override void OnInitialized()
    {
        BuilderViewModel.GetDataFromRouteParameters(this.Navigation.Uri);
    }

    private void HandleIngredientChange(ChangeEventArgs e, IngredientDto ingredient)
    {
        BuilderViewModel.ToggleIngredientSelection(ingredient);
        StateHasChanged();
    }
}