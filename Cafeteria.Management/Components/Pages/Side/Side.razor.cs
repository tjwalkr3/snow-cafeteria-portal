using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private bool ShowModal { get; set; }
    private bool IsEditMode { get; set; }
    private SideDto CurrentSide { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadSidesAsync();
    }

    private void ShowCreateModal()
    {
        CurrentSide = new SideDto();
        IsEditMode = false;
        ShowModal = true;
    }

    private void ShowEditModal(SideDto side)
    {
        CurrentSide = new SideDto
        {
            Id = side.Id,
            StationId = side.StationId,
            SideName = side.SideName,
            SideDescription = side.SideDescription,
            SidePrice = side.SidePrice,
            ImageUrl = side.ImageUrl
        };
        IsEditMode = true;
        ShowModal = true;
    }

    private void CloseModal()
    {
        ShowModal = false;
    }

    private async Task HandleSave()
    {
        ShowModal = false;
        await ViewModel.LoadSidesAsync();
    }

    private async Task DeleteSide(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this side?");
        if (confirmed)
        {
            await ViewModel.DeleteSideAsync(id);
        }
    }
}
