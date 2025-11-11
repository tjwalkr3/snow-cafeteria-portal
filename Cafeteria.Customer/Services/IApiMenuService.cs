using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public interface IApiMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<EntreeDto>> GetEntreesByStation(int stationId);
    Task<List<SideDto>> GetSidesByStation(int stationId);
    Task<List<DrinkDto>> GetDrinksByLocation(int locationId);
    Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId);
    Task<List<FoodOptionDto>> GetOptionsBySide(int sideId);
}
