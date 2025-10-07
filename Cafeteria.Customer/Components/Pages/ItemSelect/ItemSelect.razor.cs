using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.ItemSelect;

public partial class ItemSelect
{
    public bool IsInitialized { get; set; } = false;
    public List<FoodItemDto> foodItems = new();
    private bool isModalOpen = false;
    private FoodItemDto? selectedFoodItem = null;

    protected override async Task OnInitializedAsync()
    {
        await ItemSelectVM.GetDataFromRouteParameters(this.Navigation.Uri);
        foodItems = await ItemSelectVM.GetFoodItemsAsync();
        IsInitialized = true;
    }

    private void OpenFoodItemModal(FoodItemDto foodItem)
    {
        Console.WriteLine($"Opening modal for: {foodItem.ItemDescription}");
        selectedFoodItem = foodItem;
        isModalOpen = true;
        StateHasChanged();
    }

    private void CloseModal()
    {
        isModalOpen = false;
        selectedFoodItem = null;
    }

    private void HandleOrderReady(Dictionary<string, string?> orderData)
    {
        // Navigate to place-order page with the order data
        string url = QueryHelpers.AddQueryString("/place-order", orderData);
        Navigation.NavigateTo(url);
    }
}