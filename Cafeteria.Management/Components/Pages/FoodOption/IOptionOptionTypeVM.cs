using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public interface IOptionOptionTypeVM
{
    List<OptionOptionTypeDto> OptionOptionTypes { get; }
    Task InitializeOptionOptionTypesAsync();
    Task<bool> DeleteOptionOptionTypeAsync(int id);
    bool ErrorOccurred();
}
