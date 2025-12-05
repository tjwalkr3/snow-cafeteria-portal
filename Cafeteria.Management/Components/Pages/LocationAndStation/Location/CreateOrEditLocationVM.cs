using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public class CreateOrEditLocationVM : ICreateOrEditLocationVM
{
    public LocationDto SelectedLocation { get; set; } = new();

    public bool IsVisible { get; private set; }

    public event Action? OnStateChanged;

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