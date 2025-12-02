using System.Data;
using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;
using Dapper;
using Moq;
using Moq.Dapper;

namespace Cafeteria.UnitTests.Api;

public class FoodTypeServiceTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly FoodTypeService _service;

    public FoodTypeServiceTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _service = new FoodTypeService(_mockDbConnection.Object);
    }

    [Fact]
    public async Task CreateFoodType_ShouldReturnCreatedFoodType()
    {
        var foodTypeDto = new FoodOptionTypeDto
        {
            FoodOptionTypeName = "Pizza Toppings",
            NumIncluded = 2,
            MaxAmount = 5,
            FoodOptionPrice = 1.50m,
            EntreeId = 1,
            SideId = null
        };

        var expectedResult = new FoodOptionTypeDto
        {
            Id = 1,
            FoodOptionTypeName = "Pizza Toppings",
            NumIncluded = 2,
            MaxAmount = 5,
            FoodOptionPrice = 1.50m,
            EntreeId = 1,
            SideId = null
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.CreateFoodType(foodTypeDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pizza Toppings", result.FoodOptionTypeName);
        Assert.Equal(2, result.NumIncluded);
        Assert.Equal(5, result.MaxAmount);
        Assert.Equal(1.50m, result.FoodOptionPrice);
        Assert.Equal(1, result.EntreeId);
        Assert.Null(result.SideId);
    }

    [Fact]
    public async Task GetFoodTypeByID_ShouldReturnFoodType()
    {
        var expectedResult = new FoodOptionTypeDto
        {
            Id = 1,
            FoodOptionTypeName = "Pizza Toppings",
            NumIncluded = 2,
            MaxAmount = 5,
            FoodOptionPrice = 1.50m,
            EntreeId = 1,
            SideId = null
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.GetFoodTypeByID(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pizza Toppings", result.FoodOptionTypeName);
        Assert.Equal(2, result.NumIncluded);
    }

    [Fact]
    public async Task GetAllFoodTypes_ShouldReturnListOfFoodTypes()
    {
        var expectedResults = new List<FoodOptionTypeDto>
        {
            new()
            {
                Id = 1,
                FoodOptionTypeName = "Pizza Toppings",
                NumIncluded = 2,
                MaxAmount = 5,
                FoodOptionPrice = 1.50m,
                EntreeId = 1
            },
            new()
            {
                Id = 2,
                FoodOptionTypeName = "Burger Toppings",
                NumIncluded = 3,
                MaxAmount = 6,
                FoodOptionPrice = 0.75m,
                EntreeId = 2
            },
            new()
            {
                Id = 3,
                FoodOptionTypeName = "Side Add-ons",
                NumIncluded = 1,
                MaxAmount = 3,
                FoodOptionPrice = 2.00m,
                SideId = 1
            }
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QueryAsync<FoodOptionTypeDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResults);

        var result = await _service.GetAllFoodTypes();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Pizza Toppings", result[0].FoodOptionTypeName);
        Assert.Equal("Burger Toppings", result[1].FoodOptionTypeName);
        Assert.Equal("Side Add-ons", result[2].FoodOptionTypeName);
    }

    [Fact]
    public async Task UpdateFoodType_ShouldReturnUpdatedFoodType()
    {
        var foodTypeDto = new FoodOptionTypeDto
        {
            FoodOptionTypeName = "Updated Toppings",
            NumIncluded = 3,
            MaxAmount = 7,
            FoodOptionPrice = 2.00m,
            EntreeId = 1,
            SideId = null
        };

        var expectedResult = new FoodOptionTypeDto
        {
            Id = 1,
            FoodOptionTypeName = "Updated Toppings",
            NumIncluded = 3,
            MaxAmount = 7,
            FoodOptionPrice = 2.00m,
            EntreeId = 1,
            SideId = null
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionTypeDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.UpdateFoodType(1, foodTypeDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Toppings", result.FoodOptionTypeName);
        Assert.Equal(3, result.NumIncluded);
        Assert.Equal(7, result.MaxAmount);
        Assert.Equal(2.00m, result.FoodOptionPrice);
    }

    [Fact]
    public async Task DeleteFoodType_WhenSuccessful_ShouldReturnTrue()
    {
        _mockDbConnection
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(1);

        var result = await _service.DeleteFoodType(1);

        Assert.True(result);
    }
}
