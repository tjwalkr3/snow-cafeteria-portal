using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface ILocationSelectVM
{
    List<LocationDto> Locations { get; }
    void OnLocationSelected(LocationDto location);
}