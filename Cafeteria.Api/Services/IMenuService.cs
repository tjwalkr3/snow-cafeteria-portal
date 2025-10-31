using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId);
    Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId);
    Task<IngredientDtoOld> GetIngredientById(int ingredientId);
}