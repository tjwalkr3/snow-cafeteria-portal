using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Tests;

public class DummyMenuTests
{
    private DummyMenuService MenuService;

    public DummyMenuTests()
    {
        MenuService = new DummyMenuService();
    }

    [Fact]
    public async Task DummyMenuServiceReturnsDummyLocationList()
    {
        var locations = await MenuService.GetAllLocations();
        List<LocationDto> locationList = locations.ToList();
        Assert.True((locations is not null) && (locationList.Count > 0));
    }

    [Fact]
    public async Task DummyMenuServiceGetsStationsForDummyLocation()
    {
        var stations = await MenuService.GetStationsByLocation(1);
        List<StationDto> stationList = stations.ToList();
        Assert.True((stations is not null) && (stationList.Count > 0));
    }

    [Fact]
    public async Task DummyMenuServiceGetsFoodItemsForDummyStation()
    {
        var foodItems = await MenuService.GetFoodItemsByStation(1);
        List<FoodItemDtoOld> foodItemList = foodItems.ToList();
        Assert.True((foodItems is not null) && (foodItemList.Count > 0));
    }

    [Fact]
    public async Task DummyMenuServiceGetsDummyIngredientById()
    {
        var ingredient = await MenuService.GetIngredientById(1);
        IngredientDtoOld ingredientDto = ingredient;
        Assert.True(ingredientDto is not null && ingredientDto.IngredientName is not null && ingredientDto.IngredientName.Length > 0);
    }

    [Fact]
    public async Task DummyMenuServiceGetsIngredientTypesForDummyFoodItem()
    {
        var types = await MenuService.GetIngredientTypesByFoodItem(1);
        List<IngredientTypeDtoOld> typeList = types.ToList();
        Assert.True((types != null) && (typeList.Count > 0));
    }

    [Fact]
    public async Task DummyMenuServiceGetsDictOfIngredientsAndTypes()
    {
        // Arrange: Get ingredient types to test with
        var types = await MenuService.GetIngredientTypesByFoodItem(1);
        List<IngredientTypeDtoOld> typeList = types.ToList();

        // Act: call method and store result in dictionary
        var ingredientsAndTypes = await MenuService.GetIngredientsOrganizedByType(typeList);
        Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>> ingredientsAndTypesDict = ingredientsAndTypes;

        // Assert
        Assert.True(ingredientsAndTypesDict is not null && ingredientsAndTypes.Count > 0);
    }

    [Fact]
    public async Task DummyMenuServiceGetsDummyIngredientsForType()
    {
        var ingredients = await MenuService.GetIngredientsByType(1);
        List<IngredientDtoOld> ingredientList = ingredients.ToList();
        Assert.True((ingredients is not null) && (ingredientList.Count > 0));
    }
}
