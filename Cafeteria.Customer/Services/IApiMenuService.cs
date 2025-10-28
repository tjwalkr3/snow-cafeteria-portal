using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Services;

public interface IApiMenuService
{
    Task<List<LocationDtoOld>> GetAllLocations();
    Task<List<StationDtoOld>> GetStationsByLocation(int locationId);
    Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId);
    Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId);
    Task<Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>> GetIngredientsOrganizedByType(List<IngredientTypeDtoOld> types);
    Task<IngredientDtoOld> GetIngredientById(int ingredientId);
}
