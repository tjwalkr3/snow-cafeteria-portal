using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Entree;

public interface IEntreeVM
{
    List<EntreeDto> Entrees { get; set; }
    Task LoadEntrees();
    Task DeleteEntree(int id);
    Task ShowCreateModal();
    void HideModal();
}
