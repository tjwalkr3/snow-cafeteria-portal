using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Cafeteria.Management.Components.Pages.Analytics;

public partial class Analytics : ComponentBase, IAsyncDisposable
{
    [Inject] public IAnalyticsVM ViewModel { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = default!;

    private bool isLoading = true;
    private bool chartRendered = false;
    private AnalyticsPeriod selectedPeriod = AnalyticsPeriod.Week;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState != null)
        {
            var auth = await AuthenticationState;
            if (auth.User.Identity?.IsAuthenticated == true)
            {
                await ViewModel.LoadData();
            }
        }
        isLoading = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!isLoading && ViewModel.HasData)
        {
            await RenderChart();
        }
    }

    private async Task SelectPeriod(AnalyticsPeriod period)
    {
        selectedPeriod = period;
        StateHasChanged();
        await RenderChart();
    }

    private async Task RenderChart()
    {
        var entries = ViewModel.GetTopFoodForPeriod(selectedPeriod);
        var labels = entries.Select(e => e.Label).ToArray();
        var counts = entries.Select(e => e.Count).ToArray();
        var foodNames = entries.Select(e => e.FoodName).ToArray();

        await JS.InvokeVoidAsync("chartManager.renderBarChart", "topFoodChart", labels, counts, foodNames);
        chartRendered = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (chartRendered)
        {
            await JS.InvokeVoidAsync("chartManager.destroy", "topFoodChart");
        }
    }
}
