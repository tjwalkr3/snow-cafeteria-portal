using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Entree;

public interface ICreateOrEditEntreeVM
{
    EntreeDto CurrentEntree { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    Task SaveEntree();
}
