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

    public Task<List<IngredientTypeDto>> GetIngredientTypesForFoodItem(int foodItemId)
    {
        // NOTE: This currently only returns ingredient types for a dummy sandwich, regardless of what foodItemId is passed in
        List<IngredientTypeDto> ingredientTypes = new()
        {
            DummyData.CreateMeatType(),
            DummyData.CreateBreadType(),
            DummyData.CreateVegetableType()
        };
        return Task.FromResult(ingredientTypes);
    }

    public Task<Dictionary<IngredientTypeDto, List<IngredientDto>>> GetIngredientsOrganizedByType(List<IngredientTypeDto> types)
    {
        var ingredientsAndTypes = new Dictionary<IngredientTypeDto, List<IngredientDto>>
        {
            { DummyData.CreateMeatType(), new List<IngredientDto> { DummyData.CreateTurkey() } },
            { DummyData.CreateBreadType(), new List<IngredientDto> { DummyData.CreateWheatBread() } },
            { DummyData.CreateVegetableType(), new List<IngredientDto> { DummyData.CreateLettuce(), DummyData.CreateTomato() } }
        };
        return Task.FromResult(ingredientsAndTypes);
    }

    public Task<List<IngredientDto>> GetIngredientsForType(int ingredientTypeId)
    {
        throw new NotImplementedException();
    }

    public Task<IngredientDto> GetIngredientById(int ingredientId)
    {
        return Task.FromResult(DummyData.GetIngredientList.First(i => i.Id == ingredientId) ?? new IngredientDto());
    }
}