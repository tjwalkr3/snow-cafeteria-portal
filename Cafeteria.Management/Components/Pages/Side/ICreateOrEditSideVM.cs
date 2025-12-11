using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    SideDto CurrentSide { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }
    Task SaveSide();
}
