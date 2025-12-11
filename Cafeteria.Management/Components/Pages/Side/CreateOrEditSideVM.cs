using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public class CreateOrEditSideVM : ICreateOrEditSideVM
{
    private readonly ISideService _sideService;
    private readonly ISideVM _parentVM;

    public SideDto CurrentSide { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }

    public CreateOrEditSideVM(ISideService sideService, ISideVM parentVM)
    {
        _sideService = sideService;
        _parentVM = parentVM;
    }

    public async Task SaveSide()
    {
        if (!ValidateSide(_parentVM.Sides, CurrentSide))
        {
            ShowToast = true;
            ToastMessage = "A side with this name already exists in this station.";
            ToastType = Toast.ToastType.Error;
            return;
        }

        try
        {
            if (IsEditing)
            {
                await _sideService.UpdateSide(CurrentSide);
            }
            else
            {
                await _sideService.CreateSide(CurrentSide);
            }

            IsVisible = false;
            await _parentVM.LoadSides();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving side: {ex.Message}");
            throw;
        }
    }

    public bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide)
    {
        return !existingSides.Any(s =>
            s.SideName.Equals(newSide.SideName, StringComparison.OrdinalIgnoreCase) &&
            s.StationId == newSide.StationId &&
            s.Id != newSide.Id);
    }
}
