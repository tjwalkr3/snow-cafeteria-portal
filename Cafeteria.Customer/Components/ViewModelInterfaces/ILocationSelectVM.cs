using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface ILocationSelectVM
{
    List<CafeteriaLocationDto> Locations { get; }
    void OnLocationSelected(CafeteriaLocationDto location);
}