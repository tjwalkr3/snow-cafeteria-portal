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

    public void HideModal()
    {
        if (CreateOrEditVM != null)
        {
            CreateOrEditVM.IsVisible = false;
        }
    }
}
