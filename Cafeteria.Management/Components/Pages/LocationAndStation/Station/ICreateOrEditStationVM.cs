using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public interface ICreateOrEditStationVM
{
    StationDto SelectedStation { get; set; }
    bool IsVisible { get; }
    int LocationId { get; set; }
    event Action? OnStateChanged;
    event Func<Task>? OnStationSaved;
    void Show(int locationId);
    void Close();
    Task SaveAsync();
    CreateOrEditStationVM.StationModalState GetState();
}