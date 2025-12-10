using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ISideVM
{
    List<SideDto> Sides { get; set; }
    Task LoadSides();
    Task DeleteSide(int id);
    Task ShowCreateModal();
    Task ShowEditModal(int id);
    void HideModal();
}
