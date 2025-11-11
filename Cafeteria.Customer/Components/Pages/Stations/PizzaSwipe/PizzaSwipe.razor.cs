using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public partial class PizzaSwipe : ComponentBase
{
    [Inject]
    private IPizzaSwipeVM VM { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    [SupplyParameterFromQuery(Name = "station")]
    public int Station { get; set; }

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await VM.LoadDataAsync(Station, Location);
            _isLoading = false;
            StateHasChanged();
        }
    }

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

    private void SelectEntree(Cafeteria.Shared.DTOs.EntreeDto entree)
    {
        VM.SelectEntree(entree);
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

    private async Task AddToOrder()
    {
        var success = await VM.AddToOrderAsync();
        if (success)
        {
            NavigationManager.NavigateTo("/place-order");
        }
    }
}
