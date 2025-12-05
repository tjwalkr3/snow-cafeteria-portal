using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    private bool ShowModal { get; set; }
    private bool IsEditMode { get; set; }
    private SideDto CurrentSide { get; set; } = new();

    // Confirmation Modal State
    private bool ShowConfirmation { get; set; }
    private string ConfirmationTitle { get; set; } = "Confirm Delete";
    private string ConfirmationMessage { get; set; } = "Are you sure you want to delete this side?";
    private int _sideIdToDelete;

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

    private void DeleteSide(int id)
    {
        _sideIdToDelete = id;
        ShowConfirmation = true;
    }

    private async Task ConfirmDelete()
    {
        ShowConfirmation = false;
        await ViewModel.DeleteSideAsync(_sideIdToDelete);
    }

    private void CancelDelete()
    {
        ShowConfirmation = false;
    }
}
