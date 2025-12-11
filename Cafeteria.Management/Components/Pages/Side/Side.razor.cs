using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Components.Shared;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    private CreateOrEditSide? modalComponent;
    private bool ShowDeleteModal { get; set; } = false;
    private int SideIdToDelete { get; set; }
    private string DeleteMessage { get; set; } = string.Empty;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadSides();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && modalComponent != null)
        {
            modalComponent.SetParentComponent(this);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleCreateClick()
    {
        await ViewModel.ShowCreateModal();
        modalComponent?.Refresh();
    }

    private async Task HandleEditClick(int id)
    {
        await ViewModel.ShowEditModal(id);
        modalComponent?.Refresh();
    }

    private void HandleDeleteClick(int id)
    {
        var side = ViewModel.Sides.FirstOrDefault(s => s.Id == id);
        var sideName = side?.SideName ?? "this side";

        SideIdToDelete = id;
        DeleteMessage = $"Are you sure you want to delete '{sideName}'? This action cannot be undone.";
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (SideIdToDelete > 0)
        {
            var side = ViewModel.Sides.FirstOrDefault(s => s.Id == SideIdToDelete);
            var sideName = side?.SideName ?? "Side";

            try
            {
                await ViewModel.DeleteSide(SideIdToDelete);
                await RefreshSides();

                toastMessage = $"'{sideName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{sideName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                SideIdToDelete = 0;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        SideIdToDelete = 0;
    }

    private async Task RefreshSides()
    {
        await ViewModel.LoadSides();
        StateHasChanged();
    }

    public async Task RefreshSidesAfterSave()
    {
        await RefreshSides();
    }

    public void ShowSaveSuccessToast(string sideName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"'{sideName}' has been updated successfully."
            : $"'{sideName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;
        StateHasChanged();
    }

    public void ShowSaveErrorToast(string sideName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"Failed to update '{sideName}'. Please try again."
            : $"Failed to create '{sideName}'. Please try again.";
        toastType = ToastType.Error;
        showToast = true;
        StateHasChanged();
    }
}
