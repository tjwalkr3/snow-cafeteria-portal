using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public class CreateOrEditLocationVM(ILocationService locationService) : ICreateOrEditLocationVM
{
    public LocationDto SelectedLocation { get; set; } = new();

    public bool IsVisible { get; private set; }

    public event Action? OnStateChanged;
    public event Func<Task>? OnLocationSaved;

    public void Show()
    {
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
        if (GetState() == LocationModalState.Edit)
        {
            await locationService.UpdateLocation(SelectedLocation);
        }
        else
        {
            await locationService.CreateLocation(SelectedLocation);
        }
        
        if (OnLocationSaved is not null)
        {
            await OnLocationSaved.Invoke();
        }

        Close();
    }

    public enum LocationModalState
    {
        Create,
        Edit
    }

    public LocationModalState GetState()
    {
        return SelectedLocation.Id == 0 ? LocationModalState.Create : LocationModalState.Edit;
    }
}