using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.OptionOptionTypes;

public class OptionOptionTypeService(IHttpClientAuth client) : IOptionOptionTypeService
{
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await client.GetAsync<List<OptionOptionTypeDto>>("OptionOptionType") ?? [];
    }

    public async Task<OptionOptionTypeDto?> GetOptionOptionTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<OptionOptionTypeDto>($"OptionOptionType/{id}");
    }

    public async Task<bool> DeleteOptionOptionTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"OptionOptionType/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto)
    {
        var response = await client.PostAsync("OptionOptionType", optionOptionTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create option-option-type mapping");
    }
}
