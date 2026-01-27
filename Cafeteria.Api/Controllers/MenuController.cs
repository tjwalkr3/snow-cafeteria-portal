using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Api.Services;
using Cafeteria.Api.Services.Locations;
using Cafeteria.Api.Services.Stations;
using Cafeteria.Api.Services.Entrees;
using Cafeteria.Api.Services.Sides;
using Cafeteria.Api.Services.Drinks;
using Cafeteria.Api.Services.FoodOptions;
using Cafeteria.Api.Services.FoodOptionTypes;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController(
    ILocationService locationService,
    IStationService stationService,
    IEntreeService entreeService,
    ISideService sideService,
    IDrinkService drinkService,
    IFoodOptionService foodOptionService,
    IFoodOptionTypeService foodOptionTypeService
    ) : ControllerBase
{
    private readonly ILocationService _locationService = locationService;
    private readonly IStationService _stationService = stationService;
    private readonly IEntreeService _entreeService = entreeService;
    private readonly ISideService _sideService = sideService;
    private readonly IDrinkService _drinkService = drinkService;
    private readonly IFoodOptionService _foodOptionService = foodOptionService;
    private readonly IFoodOptionTypeService _foodOptionTypeService = foodOptionTypeService;

    [HttpGet("locations")]
    public async Task<List<LocationDto>> GetAllLocations()
    {
        return await _locationService.GetAllLocations();
    }

    [HttpGet("stations/location/{locationId}")]
    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return await _stationService.GetStationsByLocation(locationId);
    }

    [HttpGet("entrees")]
    public async Task<List<EntreeDto>> GetAllEntrees()
    {
        return await _entreeService.GetAllEntrees();
    }

    [HttpGet("entrees/station/{stationId}")]
    public async Task<List<EntreeDto>> GetEntreesByStation(int stationId)
    {
        return await _entreeService.GetEntreesByStationID(stationId);
    }

    [HttpGet("sides")]
    public async Task<List<SideDto>> GetAllSides()
    {
        return await _sideService.GetAllSides();
    }

    [HttpGet("sides/station/{stationId}")]
    public async Task<List<SideDto>> GetSidesByStation(int stationId)
    {
        return await _sideService.GetSidesByStationID(stationId);
    }

    [HttpGet("drinks/location/{locationId}")]
    public async Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
    {
        return await _drinkService.GetDrinksByLocationID(locationId);
    }

    [HttpGet("options/entree/{entreeId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsByEntree(int entreeId)
    {
        return await _foodOptionService.GetOptionsByEntree(entreeId);
    }

    [HttpGet("options/side/{sideId}")]
    public async Task<List<FoodOptionDto>> GetFoodOptionsBySide(int sideId)
    {
        return await _foodOptionService.GetOptionsBySide(sideId);
    }

    [HttpGet("option-types/entree/{entreeId}")]
    public async Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId)
    {
        return await _foodOptionTypeService.GetOptionTypesByEntree(entreeId);
    }

    [HttpGet("option-types-with-options/entree/{entreeId}")]
    public async Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId)
    {
        return await _foodOptionTypeService.GetOptionTypesWithOptionsByEntree(entreeId);
    }
}