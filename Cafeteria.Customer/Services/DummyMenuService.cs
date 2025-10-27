using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.Data;

namespace Cafeteria.Customer.Services;

/// <summary>
/// This dummy implementation of the IMenuService uses static dummy data from the DummyData class and mimics behavior of a MenuService.
/// </summary>
public class DummyMenuService : IApiMenuService
{
    public Task<List<LocationDtoOld>> GetAllLocations()
    {
        return Task.FromResult(DummyData.GetLocationList);
    }

    public Task<List<StationDtoOld>> GetStationsByLocation(int locationId)
    {
        return Task.FromResult(DummyData.GetStationList.Where(s => s.LocationId == locationId).ToList() ?? new List<StationDtoOld>());
    }

    public Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
    {
        return Task.FromResult(DummyData.GetFoodItemList.Where(f => f.StationId == stationId).ToList() ?? new List<FoodItemDtoOld>());
    }

    public Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
    {
        // NOTE: This currently only returns ingredient types for a dummy sandwich, regardless of what foodItemId is passed in
        List<IngredientTypeDtoOld> ingredientTypes = new()
        {
            DummyData.CreateMeatType(),
            DummyData.CreateBreadType(),
            DummyData.CreateVegetableType()
        };
        return Task.FromResult(ingredientTypes);
    }

    public Task<Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>> GetIngredientsOrganizedByType(List<IngredientTypeDtoOld> types)
    {
        var ingredientsAndTypes = new Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>
        {
            { DummyData.CreateMeatType(), new List<IngredientDtoOld> { DummyData.CreateTurkey() } },
            { DummyData.CreateBreadType(), new List<IngredientDtoOld> { DummyData.CreateWheatBread() } },
            { DummyData.CreateVegetableType(), new List<IngredientDtoOld> { DummyData.CreateLettuce(), DummyData.CreateTomato() } }
        };
        return Task.FromResult(ingredientsAndTypes);
    }

    public Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId)
    {
        // NOTE: This currently only returns ingredients for a vegetable type, regardless of what ingredientTypeId is passed in
        List<IngredientDtoOld> ingredients = new()
        {
            DummyData.CreateLettuce(),
            DummyData.CreateTomato()
        };
        return Task.FromResult(ingredients);
    }

    public Task<IngredientDtoOld> GetIngredientById(int ingredientId)
    {
        return Task.FromResult(DummyData.GetIngredientList.First(i => i.Id == ingredientId) ?? new IngredientDtoOld());
    }
}