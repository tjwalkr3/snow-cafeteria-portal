using Cafeteria.Management.Services.Icons;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Shared;

public partial class IconPicker : ComponentBase
{
    [Inject]
    private IIconService IconService { get; set; } = default!;

    [Parameter]
    public int? Value { get; set; }

    [Parameter]
    public EventCallback<int?> ValueChanged { get; set; }

    private string searchQuery = string.Empty;
    private List<IconDto> _allIcons = [];

    protected override async Task OnInitializedAsync()
    {
        _allIcons = await IconService.GetAllIcons();
    }

    private string SelectedBootstrapName =>
        _allIcons.FirstOrDefault(i => i.Id == Value)?.BootstrapName ?? string.Empty;

    private IEnumerable<IconDto> FilteredIcons =>
        string.IsNullOrWhiteSpace(searchQuery)
            ? _allIcons
            : _allIcons.Where(i =>
                i.IconName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                i.BootstrapName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

    private async Task SelectIcon(int iconId)
    {
        Value = iconId;
        await ValueChanged.InvokeAsync(iconId);
    }

    private async Task ClearIcon()
    {
        Value = null;
        await ValueChanged.InvokeAsync(null);
    }
}
