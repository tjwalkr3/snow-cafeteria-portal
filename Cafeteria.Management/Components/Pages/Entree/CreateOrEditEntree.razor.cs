using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class CreateOrEditEntree : ComponentBase
{
    [Inject]
    public IEntreeService EntreeService { get; set; } = default!;

    [Inject]
    public IEntreeVM ParentVM { get; set; } = default!;

    public ICreateOrEditEntreeVM? ViewModel { get; set; }
    private Entree? parentComponent;

    protected override void OnInitialized()
    {
        ViewModel = new CreateOrEditEntreeVM(EntreeService, ParentVM);
        if (ParentVM is EntreeVM entreeVM)
        {
            entreeVM.SetCreateOrEditVM(ViewModel);
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null)
        {
            await ViewModel.SaveEntree();
            StateHasChanged();
            if (parentComponent != null)
            {
                await parentComponent.RefreshEntreesAfterSave();
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
}
