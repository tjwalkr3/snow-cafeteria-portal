using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public class OptionOptionTypeService(IHttpClientAuth client) : IOptionOptionTypeService
{
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await client.GetAsync<List<OptionOptionTypeDto>>("api/manager/option-option-types") ?? [];
    }

    public async Task<OptionOptionTypeDto?> GetOptionOptionTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<OptionOptionTypeDto>($"api/manager/option-option-types/{id}");
    }

    public async Task<bool> DeleteOptionOptionTypeById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.DeleteAsync<object>($"api/manager/option-option-types/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<OptionOptionTypeDto> CreateOptionOptionType(OptionOptionTypeDto optionOptionTypeDto)
    {
        var response = await client.PostAsync("api/manager/option-option-types", optionOptionTypeDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>() ?? throw new InvalidOperationException("Failed to create option-option-type mapping");
    }
}
