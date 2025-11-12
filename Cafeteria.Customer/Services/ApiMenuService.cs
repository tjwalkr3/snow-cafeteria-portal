using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public class ApiMenuService(HttpClient client) : IApiMenuService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        var response = await client.GetAsync("menu/locations");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<LocationDto>>() ?? new List<LocationDto>();
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var response = await client.GetAsync($"menu/stations/location/{locationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StationDto>>() ?? [];
    }

    public async Task<List<EntreeDto>> GetEntreesByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        var response = await client.GetAsync($"menu/entrees/station/{stationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<EntreeDto>>() ?? new List<EntreeDto>();
    }

    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        var response = await client.GetAsync($"menu/sides/station/{stationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<SideDto>>() ?? new List<SideDto>();
    }

    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var response = await client.GetAsync($"menu/drinks/location/{locationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<DrinkDto>>() ?? new List<DrinkDto>();
    }

    public async Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        var response = await client.GetAsync($"menu/options/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>() ?? new List<FoodOptionDto>();
    }

    public async Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
    {
        if (sideId < 1)
            throw new ArgumentOutOfRangeException(nameof(sideId));

        var response = await client.GetAsync($"menu/options/side/{sideId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>() ?? new List<FoodOptionDto>();
    }

    public async Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        var response = await client.GetAsync($"menu/option-types/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>() ?? new List<FoodOptionTypeDto>();
    }

    public async Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        var response = await client.GetAsync($"menu/option-types-with-options/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FoodOptionTypeWithOptionsDto>>() ?? new List<FoodOptionTypeWithOptionsDto>();
    }
}