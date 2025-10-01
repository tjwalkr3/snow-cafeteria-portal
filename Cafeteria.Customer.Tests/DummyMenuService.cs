using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Interfaces;

namespace Cafeteria.Customer.Components.Data;

public class DummyMenuService : IMenuService
{
    public Task<List<LocationDto>> GetAllLocations()
    {
        throw new NotImplementedException();
    }

    public Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        throw new NotImplementedException();
    }

    public Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        throw new NotImplementedException();
    }

    public Task<List<IngredientDto>> GetIngredientsForType(int ingredientTypeId)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<IngredientTypeDto, List<IngredientDto>>> GetIngredientsOrganizedByType(List<IngredientTypeDto> types)
    {
        throw new NotImplementedException();
    }

    public Task<List<IngredientTypeDto>> GetIngredientTypesForFoodItem(int foodItemId)
    {
        throw new NotImplementedException();
    }

    public Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        throw new NotImplementedException();
    }
}