using Cafeteria.Shared.Services.Auth;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Services.Menu;

public class ApiMenuService(IHttpClientAuth client) : IApiMenuService
{
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await client.GetAsync<List<LocationDto>>("location") ?? new List<LocationDto>();
    }

    public async Task<LocationDto?> GetLocationById(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        return await client.GetAsync<LocationDto>($"location/{locationId}");
    }

    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        return await client.GetAsync<List<LocationBusinessHoursDto>>($"location/{locationId}/hours") ?? [];
    }

    public async Task<List<LocationExceptionHoursDto>> GetLocationExceptions(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        return await client.GetAsync<List<LocationExceptionHoursDto>>($"SchedulingExceptions/location/{locationId}") ?? [];
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        return await client.GetAsync<List<StationDto>>($"station/location/{locationId}") ?? [];
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        return await client.GetAsync<List<StationBusinessHoursDto>>($"station/{stationId}/hours") ?? [];
    }

    public async Task<List<StationExceptionHoursDto>> GetStationExceptions(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        return await client.GetAsync<List<StationExceptionHoursDto>>($"SchedulingExceptions/station/{stationId}") ?? [];
    }

    public async Task<List<EntreeDto>> GetEntreesByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        return await client.GetAsync<List<EntreeDto>>($"entree/station/{stationId}") ?? new List<EntreeDto>();
    }

    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        return await client.GetAsync<List<SideDto>>($"side/station/{stationId}") ?? new List<SideDto>();
    }

    public async Task<List<SideWithOptionsDto>> GetSidesWithOptionsByStation(int stationId)
    {
        if (stationId < 1)
            throw new ArgumentOutOfRangeException(nameof(stationId));

        return await client.GetAsync<List<SideWithOptionsDto>>($"side/station/{stationId}/with-options") ?? new List<SideWithOptionsDto>();
    }

    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        return await client.GetAsync<List<DrinkDto>>($"drink/location/{locationId}") ?? new List<DrinkDto>();
    }

    public async Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        return await client.GetAsync<List<FoodOptionDto>>($"foodoption/entree/{entreeId}") ?? new List<FoodOptionDto>();
    }

    public async Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
    {
        if (sideId < 1)
            throw new ArgumentOutOfRangeException(nameof(sideId));

        return await client.GetAsync<List<FoodOptionDto>>($"foodoption/side/{sideId}") ?? new List<FoodOptionDto>();
    }

    public async Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        return await client.GetAsync<List<FoodOptionTypeDto>>($"foodoptiontype/entree/{entreeId}") ?? new List<FoodOptionTypeDto>();
    }

    public async Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId)
    {
        if (entreeId < 1)
            throw new ArgumentOutOfRangeException(nameof(entreeId));

        return await client.GetAsync<List<FoodOptionTypeWithOptionsDto>>($"foodoptiontype/with-options/entree/{entreeId}") ?? new List<FoodOptionTypeWithOptionsDto>();
    }
}