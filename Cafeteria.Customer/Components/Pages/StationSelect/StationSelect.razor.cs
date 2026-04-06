using Microsoft.AspNetCore.Components;
using Cafeteria.Customer.Services.Cart;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public partial class StationSelect : ComponentBase
{
    [Inject]
    private IStationSelectVM StationSelectVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private ICartKeyService CartKeyService { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    public async Task HandleStationSelected(int stationId)
    {
        var cartKey = await CartKeyService.GetCartKey();
        var station = StationSelectVM.Stations?.FirstOrDefault(s => s.Id == stationId);
        if (station == null) return;

        await Cart.SetStation(cartKey, station.Id, station.StationName);
        Navigation.NavigateTo("/station");
    }

    public string CreateBackUrl() => "/location-select";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                var cartKey = await CartKeyService.GetCartKey();
                var order = await Cart.GetOrder(cartKey);
                int locationId = order?.Location?.Id ?? 0;
                await StationSelectVM.InitializeStations(locationId);
                IsInitialized = true;
                StateHasChanged();
            });
        }
    }
}