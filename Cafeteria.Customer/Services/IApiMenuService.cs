using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public interface IApiMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDtoOld>> GetStationsByLocation(int locationId);
    Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId);
    Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId);
    Task<Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>> GetIngredientsOrganizedByType(List<IngredientTypeDtoOld> types);
    Task<IngredientDtoOld> GetIngredientById(int ingredientId);
}
