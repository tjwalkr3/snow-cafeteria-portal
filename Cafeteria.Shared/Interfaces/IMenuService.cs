using Cafeteria.Shared.DTOs;

namespace Cafeteria.Shared.Interfaces;

public interface IMenuService
{
    #region Locations and Stations
    Task<List<LocationDto>> GetAllLocations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    #endregion

    #region Menu
    Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId);
    Task<List<IngredientTypeDto>> GetIngredientTypesForFoodItem(int foodItemId);
    Task<List<IngredientDto>> GetIngredientsForType(int ingredientTypeId);
    Task<Dictionary<IngredientTypeDto, List<IngredientDto>>> GetIngredientsOrganizedByType(List<IngredientTypeDto> types);
    Task<IngredientDto> GetIngredientById(int ingredientId);
    #endregion
}