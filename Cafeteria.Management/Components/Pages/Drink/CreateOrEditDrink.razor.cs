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
    public HttpClient HttpClient { get; set; } = default!;

    public ICreateOrEditDrinkVM? ViewModel { get; set; }
    private Drink? parentComponent;

    protected override async Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditDrinkVM(DrinkService, ParentVM, HttpClient);
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
        if (ViewModel != null)
        {
            await ViewModel.SaveDrink();
            StateHasChanged();
            if (parentComponent != null)
            {
                await parentComponent.RefreshDrinksAfterSave();
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
