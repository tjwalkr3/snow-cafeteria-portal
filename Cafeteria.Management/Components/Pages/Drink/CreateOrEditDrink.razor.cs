using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class CreateOrEditDrink : ComponentBase
{
    [Inject]
    public IDrinkService DrinkService { get; set; } = default!;

    [Inject]
    public IDrinkVM ParentVM { get; set; } = default!;

    public ICreateOrEditDrinkVM? ViewModel { get; set; }

    protected override void OnInitialized()
    {
        ViewModel = new CreateOrEditDrinkVM(DrinkService, ParentVM);
        if (ParentVM is DrinkVM drinkVM)
        {
            drinkVM.SetCreateOrEditVM(ViewModel);
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null)
        {
            await ViewModel.SaveDrink();
            StateHasChanged();
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
}
