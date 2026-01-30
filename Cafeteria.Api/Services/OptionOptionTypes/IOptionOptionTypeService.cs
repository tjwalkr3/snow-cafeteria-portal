using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.OptionOptionTypes;

public interface IOptionOptionTypeService
{
    Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto);
    Task<OptionOptionTypeDto?> GetOptionOptionTypeById(int id);
    Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes();
    Task<OptionOptionTypeDto?> UpdateOptionOptionTypeById(int id, OptionOptionTypeDto optionOptionTypeDto);
    Task<bool> DeleteOptionOptionTypeById(int id);
}
