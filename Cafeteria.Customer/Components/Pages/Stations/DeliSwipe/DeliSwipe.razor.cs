using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;

public partial class DeliSwipe : ComponentBase
{
    [Inject]
    private IDeliSwipeVM VM { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    [SupplyParameterFromQuery(Name = "station")]
    public int Station { get; set; }

    public string CreateBackUrl()
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", Location.ToString());

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

    private void SetActiveTab(string tab)
    {
        VM.SetActiveTab(tab);
        StateHasChanged();
    }

    private void SelectSide(Cafeteria.Shared.DTOs.SideDto side)
    {
        VM.SelectSide(side);
        StateHasChanged();
    }

    private void SelectDrink(Cafeteria.Shared.DTOs.DrinkDto drink)
    {
        VM.SelectDrink(drink);
        StateHasChanged();
    }

    private void ToggleTopping(string topping)
    {
        VM.ToggleTopping(topping);
        StateHasChanged();
    }

    private void SetBread(string bread)
    {
        VM.SetBread(bread);
        StateHasChanged();
    }

    private void SetMeat(string meat)
    {
        VM.SetMeat(meat);
        StateHasChanged();
    }

    private void SetCheese(string cheese)
    {
        VM.SetCheese(cheese);
        StateHasChanged();
    }

    private void SetDressing(string dressing)
    {
        VM.SetDressing(dressing);
        StateHasChanged();
    }

    private void AddToOrder()
    {
        VM.AddToOrder();
        StateHasChanged();
    }

    private void ClearOrderConfirmation()
    {
        VM.ClearOrderConfirmation();
        StateHasChanged();
    }
}
