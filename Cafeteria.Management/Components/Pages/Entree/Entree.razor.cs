using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Components.Shared;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class Entree : ComponentBase
{
    [Inject]
    public IEntreeVM ViewModel { get; set; } = default!;

    private CreateOrEditEntree? modalComponent;
    private bool ShowDeleteModal { get; set; } = false;
    private int EntreeIdToDelete { get; set; }
    private string DeleteMessage { get; set; } = string.Empty;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadEntrees();
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
        var entree = ViewModel.Entrees.FirstOrDefault(e => e.Id == id);
        var entreeName = entree?.EntreeName ?? "this entree";

        EntreeIdToDelete = id;
        DeleteMessage = $"Are you sure you want to delete '{entreeName}'? This action cannot be undone.";
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (EntreeIdToDelete > 0)
        {
            var entree = ViewModel.Entrees.FirstOrDefault(e => e.Id == EntreeIdToDelete);
            var entreeName = entree?.EntreeName ?? "Entree";

            try
            {
                await ViewModel.DeleteEntree(EntreeIdToDelete);
                await RefreshEntrees();

                toastMessage = $"'{entreeName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{entreeName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                EntreeIdToDelete = 0;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        EntreeIdToDelete = 0;
    }

    private async Task RefreshEntrees()
    {
        await ViewModel.LoadEntrees();
        StateHasChanged();
    }

    public async Task RefreshEntreesAfterSave()
    {
        await RefreshEntrees();
    }

    public void ShowSaveSuccessToast(string entreeName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"'{entreeName}' has been updated successfully."
            : $"'{entreeName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;
        StateHasChanged();
    }

    public void ShowSaveErrorToast(string entreeName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"Failed to update '{entreeName}'. Please try again."
            : $"Failed to create '{entreeName}'. Please try again.";
        toastType = ToastType.Error;
        showToast = true;
        StateHasChanged();
    }
}
