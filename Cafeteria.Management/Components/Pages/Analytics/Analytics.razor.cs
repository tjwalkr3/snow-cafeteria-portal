using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Pages.Analytics;

public partial class Analytics : ComponentBase
{
    [Inject]
    public IAnalyticsVM ViewModel { get; set; } = default!;

    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadData();
        isLoading = false;
    }
}
