using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public interface ICreateOrEditLocationVM
{
    LocationDto SelectedLocation { get; set; }
    bool IsVisible { get; }
    event Action? OnStateChanged;
    event Func<Task>? OnLocationSaved;
    void Show();
    void Close();
    Task SaveAsync();
    CreateOrEditLocationVM.LocationModalState GetState();
}