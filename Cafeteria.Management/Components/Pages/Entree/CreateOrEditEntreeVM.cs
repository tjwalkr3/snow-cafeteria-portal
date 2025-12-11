using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Entree;

public class CreateOrEditEntreeVM : ICreateOrEditEntreeVM
{
    private readonly IEntreeService _entreeService;
    private readonly IEntreeVM _parentVM;
    private readonly IStationService _stationService;

    public EntreeDto CurrentEntree { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }
    public List<StationDto> Stations { get; set; } = [];

    public CreateOrEditEntreeVM(IEntreeService entreeService, IEntreeVM parentVM, IStationService stationService)
    {
        _entreeService = entreeService;
        _parentVM = parentVM;
        _stationService = stationService;
    }

    public async Task LoadStations()
    {
        try
        {
            Stations = await _stationService.GetAllStations();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading stations: {ex.Message}");
        }
    }

    public bool ValidateEntree(IEnumerable<EntreeDto> existingEntrees, EntreeDto newEntree)
    {
        return !existingEntrees.Any(e =>
            e.EntreeName.Equals(newEntree.EntreeName, StringComparison.OrdinalIgnoreCase) &&
            e.StationId == newEntree.StationId &&
            e.Id != newEntree.Id);
    }

    public async Task<bool> SaveEntree()
    {
        if (!ValidateEntree(_parentVM.Entrees, CurrentEntree))
        {
            ShowToast = true;
            ToastMessage = "An entree with this name already exists in this station.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

        try
        {
            if (IsEditing)
            {
                await _entreeService.UpdateEntreeById(CurrentEntree.Id, CurrentEntree);
            }
            else
            {
                await _entreeService.CreateEntree(CurrentEntree);
            }

            IsVisible = false;
            await _parentVM.LoadEntrees();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving entree: {ex.Message}");
            throw;
        }
    }
}
