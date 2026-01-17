using Cafeteria.Customer.Components.Pages.Stations.Configuration;
using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.Stations.GenericSwipe;

public partial class GenericSwipe : ComponentBase
{
    [Inject]
    private IGenericSwipeVM VM { get; set; } = default!;

    [Inject]
    private IStationConfigurationProvider ConfigProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public string? StationType { get; set; }

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
            var stationType = DetermineStationType();
            bool isCardOrder = Payment == "card";

            await VM.InitializeAsync(stationType, Station, Location, isCardOrder);
            _isLoading = false;
            StateHasChanged();
        }
    }

    private Configuration.StationType DetermineStationType()
    {
        // First check route parameter
        if (!string.IsNullOrEmpty(StationType) && ConfigProvider.TryParseStationType(StationType, out var parsedType))
        {
            return parsedType;
        }

        // Fall back to legacy route detection
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var path = uri.AbsolutePath.ToLowerInvariant().TrimStart('/');

        return path switch
        {
            "breakfast" => Configuration.StationType.Breakfast,
            "deli" => Configuration.StationType.Deli,
            "grill" => Configuration.StationType.Grill,
            "pizza" => Configuration.StationType.Pizza,
            _ => Configuration.StationType.Grill // Default fallback
        };
    }

    private string GetStationIcon()
    {
        return VM.CurrentStationType switch
        {
            Configuration.StationType.Grill => "bi-fire",
            Configuration.StationType.Breakfast => "bi-egg-fried",
            Configuration.StationType.Pizza => "bi-basket-fill",
            Configuration.StationType.Deli => "bi-cup-straw",
            _ => "bi-shop"
        };
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

    private async Task SelectEntree(EntreeDto entree)
    {
        await VM.SelectEntreeAsync(entree);
        StateHasChanged();
    }

    private void SelectSide(SideDto side)
    {
        VM.SelectSide(side);
        StateHasChanged();
    }

    private void SelectDrink(DrinkDto drink)
    {
        VM.SelectDrink(drink);
        StateHasChanged();
    }

    private void SetOptionForType(int optionTypeId, string optionName)
    {
        VM.SetOptionForType(optionTypeId, optionName);
        StateHasChanged();
    }

    private void ToggleOptionForType(int optionTypeId, string optionName)
    {
        VM.ToggleOptionForType(optionTypeId, optionName);
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
            Dictionary<string, string?> queryParameters = new() { };

            if (!string.IsNullOrEmpty(Payment))
                queryParameters.Add("payment", Payment);

            if (Location > 0)
                queryParameters.Add("location", Location.ToString());

            string url = QueryHelpers.AddQueryString("/place-order", queryParameters);
            NavigationManager.NavigateTo(url);
        }
    }
}
