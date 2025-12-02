using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase
{
    [Parameter]
    public LocationDto SelectedLocation { get; set; } = new();

    private bool _isVisible;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public void Show()
    {
        _isVisible = true;
        StateHasChanged();
    }

    public void Close()
    {
        _isVisible = false;
        StateHasChanged();
    }

    public enum LocationModalState
    {
        Create,
        Edit
    }

    public LocationModalState GetState()
    {
        return SelectedLocation.Id == 0 ? LocationModalState.Create : LocationModalState.Edit;
    }
}
