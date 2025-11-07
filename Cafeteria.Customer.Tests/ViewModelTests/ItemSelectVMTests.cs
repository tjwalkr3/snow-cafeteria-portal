using Cafeteria.Customer.Components.Pages.ItemSelect;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.ViewModels.Tests;

public class ItemSelectVMTests
{
    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedStation_ReturnsTrue_WhenUrlParsingFailedIsTrue()
    {
        // Arrange
        var itemSelectVM = new ItemSelectVM(new FakeMenuService()); // menu service isn't needed in this test

        // Act
        await itemSelectVM.GetDataFromRouteParameters(""); // pass a url with no query string so parsing fails
        bool result = itemSelectVM.ErrorOccurredWhileParsingUrlParameters();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ErrorOccurredWhileParsingSelectedStation_ReturnsFalse_WhenUrlParsingFailedIsFalse()
    {
        // Arrange
        var itemSelectVM = new ItemSelectVM(new FakeMenuService()); // menu service isn't needed in this test

        // Act
        // Don't parse; error occurred should be false by default
        bool result = itemSelectVM.ErrorOccurredWhileParsingUrlParameters();

        // Assert
        Assert.False(result);
    }

}

internal class FakeMenuService : IApiMenuService
{
  public Task<List<LocationDto>> GetAllLocations()
  {
    throw new NotImplementedException();
  }

  public Task<List<DrinkDto>> GetDrinksByLocation(int locationId)
  {
    throw new NotImplementedException();
  }

  public Task<List<EntreeDto>> GetEntreesByStation(int stationId)
  {
    throw new NotImplementedException();
  }

  public Task<List<FoodItemDtoOld>> GetFoodItemsByStation(int stationId)
  {
    throw new NotImplementedException();
  }

  public Task<IngredientDtoOld> GetIngredientById(int ingredientId)
  {
    throw new NotImplementedException();
  }

  public Task<List<IngredientDtoOld>> GetIngredientsByType(int ingredientTypeId)
  {
    throw new NotImplementedException();
  }

  public Task<Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>> GetIngredientsOrganizedByType(List<IngredientTypeDtoOld> types)
  {
    throw new NotImplementedException();
  }

  public Task<List<IngredientTypeDtoOld>> GetIngredientTypesByFoodItem(int foodItemId)
  {
    throw new NotImplementedException();
  }

  public Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId)
  {
    throw new NotImplementedException();
  }

  public Task<List<FoodOptionDto>> GetOptionsBySide(int sideId)
  {
    throw new NotImplementedException();
  }

  public Task<List<SideDto>> GetSidesByStation(int stationId)
  {
    throw new NotImplementedException();
  }

  public Task<List<StationDto>> GetStationsByLocation(int locationId)
  {
    throw new NotImplementedException();
  }
}