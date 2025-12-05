using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<EntreeDto>> GetAllEntrees();
    Task<List<EntreeDto>> GetEntreesByStation(int stationId);
    Task<List<SideDto>> GetAllSides();
    Task<List<SideDto>> GetSidesByStation(int stationId);
    Task<List<DrinkDto>> GetDrinksByLocation(int locationId);
    Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId);
    Task<List<FoodOptionDto>> GetOptionsBySide(int sideId);
    Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId);
    Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId);
}