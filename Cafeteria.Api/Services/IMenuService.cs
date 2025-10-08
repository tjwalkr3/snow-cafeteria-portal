using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IMenuService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDto>> GetIngredientTypesByFoodItem(int foodItemId);
    Task<List<IngredientDto>> GetIngredientsByType(int ingredientTypeId);
    Task<IngredientDto> GetIngredientById(int ingredientId);
}