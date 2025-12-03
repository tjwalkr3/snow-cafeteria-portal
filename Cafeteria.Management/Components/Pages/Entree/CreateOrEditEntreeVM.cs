using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Entree;

public class CreateOrEditEntreeVM : ICreateOrEditEntreeVM
{
    private readonly IEntreeService _entreeService;
    private readonly IEntreeVM _parentVM;

    public EntreeDto CurrentEntree { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }

    public CreateOrEditEntreeVM(IEntreeService entreeService, IEntreeVM parentVM)
    {
        _entreeService = entreeService;
        _parentVM = parentVM;
    }

    public async Task SaveEntree()
    {
        try
        {
            await _entreeService.CreateEntree(CurrentEntree);

            IsVisible = false;
            await _parentVM.LoadEntrees();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving entree: {ex.Message}");
            throw;
        }
    }
}
