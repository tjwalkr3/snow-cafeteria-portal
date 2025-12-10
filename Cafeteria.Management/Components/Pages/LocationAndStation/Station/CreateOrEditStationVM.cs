using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public class CreateOrEditStationVM(IStationService stationService) : ICreateOrEditStationVM
{
    public StationDto SelectedStation { get; set; } = new();
    public int LocationId { get; set; }

    public bool IsVisible { get; private set; }

    public event Action? OnStateChanged;
    public event Func<Task>? OnStationSaved;

    public void Show(int locationId)
    {
        LocationId = locationId;
        IsVisible = true;
        OnStateChanged?.Invoke();
    }

    public void Close()
    {
        IsVisible = false;
        OnStateChanged?.Invoke();
    }

    public async Task SaveAsync()
    {
        if (GetState() == StationModalState.Edit)
        {
            await stationService.UpdateStation(SelectedStation);
        }
        else
        {
            await stationService.CreateStation(LocationId, SelectedStation);
        }
        
        if (OnStationSaved is not null)
        {
            await OnStationSaved.Invoke();
        }

        Close();
    }

    public enum StationModalState
    {
        Create,
        Edit
    }

    public StationModalState GetState()
    {
        return SelectedStation.Id == 0 ? StationModalState.Create : StationModalState.Edit;
    }
}