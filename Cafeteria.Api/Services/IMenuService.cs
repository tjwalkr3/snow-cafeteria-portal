using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Api.Services;

public interface IMenuService
{
    Task<List<LocationDtoOld>> GetAllLocations();
    Task<List<StationDtoOld>> GetStationsByLocation(int locationId);
    Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId);
    Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId);
    Task<IngredientDtoOld> GetIngredientById(int ingredientId);
}