using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IOptionOptionTypeService
{
    Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto);
    Task<OptionOptionTypeDto?> GetOptionOptionTypeByID(int id);
    Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes();
    Task<OptionOptionTypeDto?> UpdateOptionOptionTypeById(int id, OptionOptionTypeDto optionOptionTypeDto);
    Task<bool> DeleteOptionOptionTypeById(int id);
}
