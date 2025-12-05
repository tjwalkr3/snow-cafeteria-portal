using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IOptionOptionTypeService
{
    Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes();
    Task<OptionOptionTypeDto?> GetOptionOptionTypeById(int id);
    Task<bool> DeleteOptionOptionTypeById(int id);
    Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto);
}
