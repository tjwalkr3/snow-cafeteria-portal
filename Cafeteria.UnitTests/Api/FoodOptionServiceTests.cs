using System.Data;
using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;
using Dapper;
using Moq;
using Moq.Dapper;

namespace Cafeteria.UnitTests.Api;

public class FoodOptionServiceTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly FoodOptionService _service;

    public FoodOptionServiceTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _service = new FoodOptionService(_mockDbConnection.Object);
    }

    [Fact]
    public async Task CreateFoodOption_ShouldReturnCreatedFoodOption()
    {
        var foodOptionDto = new FoodOptionDto
        {
            FoodOptionName = "Pepperoni",
            InStock = true,
            ImageUrl = "https://example.com/pepperoni.jpg"
        };

        var expectedResult = new FoodOptionDto
        {
            Id = 1,
            FoodOptionName = "Pepperoni",
            InStock = true,
            ImageUrl = "https://example.com/pepperoni.jpg"
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.CreateFoodOption(foodOptionDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pepperoni", result.FoodOptionName);
        Assert.True(result.InStock);
        Assert.Equal("https://example.com/pepperoni.jpg", result.ImageUrl);
    }

    [Fact]
    public async Task GetFoodOptionByID_ShouldReturnFoodOption()
    {
        var expectedResult = new FoodOptionDto
        {
            Id = 1,
            FoodOptionName = "Pepperoni",
            InStock = true,
            ImageUrl = "https://example.com/pepperoni.jpg"
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.GetFoodOptionByID(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Pepperoni", result.FoodOptionName);
    }

    [Fact]
    public async Task GetAllFoodOptions_ShouldReturnListOfFoodOptions()
    {
        var expectedResults = new List<FoodOptionDto>
        {
            new() { Id = 1, FoodOptionName = "Pepperoni", InStock = true },
            new() { Id = 2, FoodOptionName = "Mushrooms", InStock = true },
            new() { Id = 3, FoodOptionName = "Extra Cheese", InStock = false }
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QueryAsync<FoodOptionDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResults);

        var result = await _service.GetAllFoodOptions();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Pepperoni", result[0].FoodOptionName);
        Assert.Equal("Mushrooms", result[1].FoodOptionName);
        Assert.Equal("Extra Cheese", result[2].FoodOptionName);
    }

    [Fact]
    public async Task UpdateFoodOption_ShouldReturnUpdatedFoodOption()
    {
        var foodOptionDto = new FoodOptionDto
        {
            FoodOptionName = "Updated Pepperoni",
            InStock = false,
            ImageUrl = "https://example.com/updated.jpg"
        };

        var expectedResult = new FoodOptionDto
        {
            Id = 1,
            FoodOptionName = "Updated Pepperoni",
            InStock = false,
            ImageUrl = "https://example.com/updated.jpg"
        };

        _mockDbConnection
            .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<FoodOptionDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(expectedResult);

        var result = await _service.UpdateFoodOption(1, foodOptionDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Pepperoni", result.FoodOptionName);
        Assert.False(result.InStock);
    }

    [Fact]
    public async Task DeleteFoodOption_WhenSuccessful_ShouldReturnTrue()
    {
        _mockDbConnection
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()))
            .ReturnsAsync(1);

        var result = await _service.DeleteFoodOption(1);

        Assert.True(result);
    }
}
