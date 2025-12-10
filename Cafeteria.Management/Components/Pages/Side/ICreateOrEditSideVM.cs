using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    SideDto CurrentSide { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    Task SaveSide();
}
