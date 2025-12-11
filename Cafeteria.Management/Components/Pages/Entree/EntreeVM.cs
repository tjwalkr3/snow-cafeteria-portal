using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Entree;

public class EntreeVM : IEntreeVM
{
    private readonly IEntreeService _entreeService;
    public ICreateOrEditEntreeVM? CreateOrEditVM { get; set; }

    public List<EntreeDto> Entrees { get; set; } = [];

    public EntreeVM(IEntreeService entreeService)
    {
        _entreeService = entreeService;
    }

    public void SetCreateOrEditVM(ICreateOrEditEntreeVM vm)
    {
        CreateOrEditVM = vm;
    }

    public async Task LoadEntrees()
    {
        try
        {
            Entrees = await _entreeService.GetAllEntrees();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading entrees: {ex.Message}");
        }
    }

    public async Task DeleteEntree(int id)
    {
        try
        {
            await _entreeService.DeleteEntreeById(id);
            await LoadEntrees();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting entree: {ex.Message}");
            throw;
        }
    }

    public Task ShowCreateModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.CurrentEntree = new EntreeDto();
            CreateOrEditVM.IsEditing = false;
            CreateOrEditVM.IsVisible = true;
        }
        return Task.CompletedTask;
    }

    public async Task ShowEditModal(int id)
    {
        if (CreateOrEditVM != null)
        {
            var entree = await _entreeService.GetEntreeById(id);
            if (entree != null)
            {
                CreateOrEditVM.CurrentEntree = entree;
                CreateOrEditVM.IsEditing = true;
                CreateOrEditVM.IsVisible = true;
            }
        }
    }

    public async Task ToggleStockStatus(int id, bool inStock)
    {
        try
        {
            await _entreeService.SetStockStatusById(id, inStock);
            var entree = Entrees.FirstOrDefault(e => e.Id == id);
            if (entree != null)
            {
                entree.InStock = inStock;
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
