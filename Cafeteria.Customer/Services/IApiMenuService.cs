using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public interface IApiMenuService
{
    List<LocationDto> GetAllLocations();
    List<StationDto> GetStationsByLocation(int locationId);
    List<FoodItemDto> GetFoodItemsByStation(int stationId);
    List<IngredientTypeDto> GetIngredientTypesByFoodItem(int foodItemId);
    List<IngredientDto> GetIngredientsByType(int ingredientTypeId);
}
