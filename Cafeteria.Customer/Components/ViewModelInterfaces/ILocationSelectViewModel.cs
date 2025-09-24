using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface ILocationSelectViewModel
{
    List<CafeteriaLocationDto> Locations { get; }
    void OnLocationSelected(CafeteriaLocationDto location);
}