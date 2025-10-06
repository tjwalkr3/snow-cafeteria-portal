using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.FoodItemBuilder;

public partial class FoodItemBuilder
{
    public bool IsInitialized { get; set; } = false;
    protected override async Task OnInitializedAsync()
    {
        await BuilderViewModel.GetDataFromRouteParameters(this.Navigation.Uri);
        IsInitialized = true;
    }

    private void HandleIngredientChange(ChangeEventArgs e, IngredientDto ingredient)
    {
        BuilderViewModel.ToggleIngredientSelection(ingredient);
    }
}