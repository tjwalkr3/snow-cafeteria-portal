using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class CreateOrEditDrink : ComponentBase
{
    [Inject]
    public IDrinkService DrinkService { get; set; } = default!;

    [Inject]
    public IDrinkVM ParentVM { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    public ICreateOrEditDrinkVM? ViewModel { get; set; }
    private Drink? parentComponent;

    protected override async Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditDrinkVM(DrinkService, ParentVM, StationService);
        if (ParentVM is DrinkVM drinkVM)
        {
            drinkVM.SetCreateOrEditVM(ViewModel);
        }

        if (ViewModel is CreateOrEditDrinkVM vm)
        {
            await vm.LoadStations();
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null && parentComponent != null)
        {
            var drinkName = ViewModel.CurrentDrink.DrinkName ?? "Drink";
            var isEdit = ViewModel.IsEditing;
            
            try
            {
                await ViewModel.SaveDrink();
                StateHasChanged();
                await parentComponent.RefreshDrinksAfterSave();
                parentComponent.ShowSaveSuccessToast(drinkName, isEdit);
            }
            catch
            {
                parentComponent.ShowSaveErrorToast(drinkName, isEdit);
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

    public void SetParentComponent(Drink parent)
    {
        parentComponent = parent;
    }
}
