using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Side;

public class SideVM : ISideVM
{
    private readonly ISideService _sideService;
    public ICreateOrEditSideVM? CreateOrEditVM { get; set; }

    public List<SideDto> Sides { get; set; } = [];

    public SideVM(ISideService sideService)
    {
        _sideService = sideService;
    }

    public void SetCreateOrEditVM(ICreateOrEditSideVM vm)
    {
        CreateOrEditVM = vm;
    }

    public async Task LoadSides()
    {
        try
        {
            Sides = await _sideService.GetAllSides();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading sides: {ex.Message}");
        }
    }

    public async Task DeleteSide(int id)
    {
        try
        {
            await _sideService.DeleteSide(id);
            await LoadSides();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting side: {ex.Message}");
            throw;
        }
    }

    public Task ShowCreateModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.CurrentSide = new SideDto();
            CreateOrEditVM.IsEditing = false;
            CreateOrEditVM.IsVisible = true;
        }
        return Task.CompletedTask;
    }

    public async Task ShowEditModal(int id)
    {
        if (CreateOrEditVM != null)
        {
            var side = await _sideService.GetSideById(id);
            if (side != null)
            {
                CreateOrEditVM.CurrentSide = side;
                CreateOrEditVM.IsEditing = true;
                CreateOrEditVM.IsVisible = true;
            }
        }
    }

    public async Task ToggleStockStatus(int id, bool inStock)
    {
        try
        {
            await _sideService.SetStockStatusById(id, inStock);
            var side = Sides.FirstOrDefault(s => s.Id == id);
            if (side != null)
            {
                side.InStock = inStock;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error toggling stock status: {ex.Message}");
        }
    }

    public void HideModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.IsVisible = false;
        }
    }
}
