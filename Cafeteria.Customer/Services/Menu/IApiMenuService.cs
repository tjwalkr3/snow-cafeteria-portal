using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Services.Menu;

public interface IApiMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<LocationDto?> GetLocationById(int locationId);
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
    Task<List<LocationExceptionHoursDto>> GetLocationExceptions(int locationId);
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<List<StationExceptionHoursDto>> GetStationExceptions(int stationId);
    Task<List<EntreeDto>> GetEntreesByStation(int stationId);
    Task<List<EntreeDto>> GetCardEntreesByStation(int stationId);
    Task<List<EntreeDto>> GetSwipeEntreesByStation(int stationId);
    Task<List<SideWithOptionsDto>> GetSidesByStation(int stationId);
    Task<List<SideWithOptionsDto>> GetCardSidesByStation(int stationId);
    Task<List<SideWithOptionsDto>> GetSwipeSidesByStation(int stationId);
    Task<List<SideWithOptionsDto>> GetSidesWithOptionsByStation(int stationId);
    Task<List<DrinkDto>> GetDrinksByLocation(int locationId);
    Task<List<DrinkDto>> GetCardDrinksByLocation(int locationId);
    Task<List<DrinkDto>> GetSwipeDrinksByLocation(int locationId);
    Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId);
    Task<List<FoodOptionDto>> GetOptionsBySide(int sideId);
    Task<List<FoodOptionTypeDto>> GetOptionTypesByEntree(int entreeId);
    Task<List<FoodOptionTypeWithOptionsDto>> GetOptionTypesWithOptionsByEntree(int entreeId);
}
