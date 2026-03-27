using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services.Entrees;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class CreateOrEditEntree : ComponentBase
{
    [Inject]
    public IEntreeService EntreeService { get; set; } = default!;

    [Inject]
    public IEntreeVM ParentVM { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    public ICreateOrEditEntreeVM? ViewModel { get; set; }
    private Entree? parentComponent;

    protected override async Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditEntreeVM(EntreeService, ParentVM, StationService);
        if (ParentVM is EntreeVM entreeVM)
        {
            entreeVM.SetCreateOrEditVM(ViewModel);
        }

        if (ViewModel is CreateOrEditEntreeVM vm)
        {
            await vm.LoadStations();
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null && parentComponent != null)
        {
            var entreeName = ViewModel.CurrentEntree.EntreeName ?? "Entree";
            var isEdit = ViewModel.IsEditing;

            try
            {
                var success = await ViewModel.SaveEntree();
                if (success)
                {
                    StateHasChanged();
                    await parentComponent.RefreshEntreesAfterSave();
                    parentComponent.ShowSaveSuccessToast(entreeName, isEdit);
                }
            }
            catch
            {
                parentComponent.ShowSaveErrorToast(entreeName, isEdit);
            }
        }
    }

    private void HandleClose()
    {
        if (ViewModel != null)
        {
            ViewModel.IsVisible = false;
            StateHasChanged();
        }
    }

    public void Refresh()
    {
        StateHasChanged();
    }

    public void SetParentComponent(Entree parent)
    {
        parentComponent = parent;
    }

    private void SetItemType(EntreeDto entree, string itemType)
    {
        entree.CardOnly = false;
        entree.SwipeOnly = false;

        switch (itemType)
        {
            case "cardonly":
                entree.CardOnly = true;
                break;
            case "swipeonly":
                entree.SwipeOnly = true;
                break;
            case "both":
            default:
                break;
        }
    }
}
