using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    SideDto CurrentSide { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    Task SaveSide();
}
