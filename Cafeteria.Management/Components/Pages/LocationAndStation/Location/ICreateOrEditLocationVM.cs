using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public interface ICreateOrEditLocationVM
{
    LocationDto SelectedLocation { get; set; }
    bool IsVisible { get; }
    void Show();
    void Close();
    CreateOrEditLocationVM.LocationModalState GetState();
}