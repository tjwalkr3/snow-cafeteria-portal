using System.ComponentModel.DataAnnotations;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Shared;

public class MenuDtoValidationTests
{
    [Fact]
    public void EntreeDto_AllowsZeroPrice()
    {
        var dto = new EntreeDto
        {
            StationId = 1,
            EntreeName = "Test Entree",
            EntreePrice = 0m
        };

        Assert.True(IsValid(dto));
    }

    [Fact]
    public void SideDto_AllowsZeroPrice()
    {
        var dto = new SideDto
        {
            StationId = 1,
            SideName = "Test Side",
            SidePrice = 0m
        };

        Assert.True(IsValid(dto));
    }

    [Fact]
    public void DrinkDto_AllowsZeroPrice()
    {
        var dto = new DrinkDto
        {
            LocationId = 1,
            DrinkName = "Test Drink",
            DrinkPrice = 0m
        };

        Assert.True(IsValid(dto));
    }

    private static bool IsValid<T>(T model)
    {
        var context = new ValidationContext(model!);
        var results = new List<ValidationResult>();
        return Validator.TryValidateObject(model!, context, results, validateAllProperties: true);
    }
}
