using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.FoodItemBuilderModal;

public partial class FoodItemBuilder
{
    [Parameter]
    public FoodItemDtoOld? FoodItem { get; set; }

    [Parameter]
    public EventCallback<Dictionary<string, string?>> OnOrderReady { get; set; }

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        // Support both parameter-based initialization (modal) and route-based (legacy)
        if (FoodItem != null)
        {
            await BuilderViewModel.InitializeWithFoodItem(FoodItem);
        }
        else
        {
            await BuilderViewModel.GetDataFromRouteParameters(this.Navigation.Uri);
        }
        IsInitialized = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (FoodItem != null && IsInitialized)
        {
            await BuilderViewModel.InitializeWithFoodItem(FoodItem);
        }
    }

    private void HandleIngredientChange(ChangeEventArgs e, IngredientDtoOld ingredient)
    {
        BuilderViewModel.ToggleIngredientSelection(ingredient);
    }

    private async Task HandleOrderSubmit()
    {
        var orderData = BuilderViewModel.GetOrderAsJson();
        await OnOrderReady.InvokeAsync(orderData);
    }
}