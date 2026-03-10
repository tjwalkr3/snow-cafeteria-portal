using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Customer.Components.Pages.Stations.Panels.DrinkPanel;

public partial class DrinkPanel : ComponentBase
{
    [Parameter, EditorRequired]
    public List<DrinkDto> Drinks { get; set; } = new();

    [Parameter, EditorRequired]
    public DrinkDto? SelectedDrink { get; set; }

    [Parameter, EditorRequired]
    public bool IsCardOrder { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<DrinkDto> OnDrinkSelected { get; set; }
}
