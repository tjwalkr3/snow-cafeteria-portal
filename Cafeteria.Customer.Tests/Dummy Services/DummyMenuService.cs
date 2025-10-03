using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Interfaces;
using Cafeteria.Customer.Components.Data;

namespace Cafeteria.Customer.Tests.DummyServices;

/// <summary>
/// This dummy implementation of the IMenuService uses static dummy data from the DummyData class and mimics behavior of a MenuService.
/// </summary>
public class DummyMenuService : IMenuService
{
    public Task<List<LocationDto>> GetAllLocations()
    {
        return Task.FromResult(DummyData.GetLocationList);
    }

    public Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        return Task.FromResult(DummyData.GetStationList.Where(s => s.LocationId == locationId).ToList() ?? new List<StationDto>());
    }

    public Task<List<FoodItemDto>> GetFoodItemsByStation(int stationId)
    {
        return Task.FromResult(DummyData.GetFoodItemList.Where(f => f.StationId == stationId).ToList() ?? new List<FoodItemDto>());
    }

    public Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        return Task.FromResult(DummyData.GetIngredientList.First(i => i.Id == ingredientId) ?? new IngredientDto());
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
}