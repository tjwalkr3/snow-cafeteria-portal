using Cafeteria.Management.Components.Pages.Drink;
using Cafeteria.Management.Components.Pages.Entree;
using Cafeteria.Management.Components.Pages.Side;
using Cafeteria.Shared.DTOs;
using Moq;
using Xunit;

namespace Cafeteria.UnitTests.Management.Components.Pages;

public class ValidationTests
{
    [Fact]
    public void ValidateEntree_ReturnsFalse_WhenDuplicateExists()
    {
        // Arrange
        var vm = new CreateOrEditEntreeVM(null!, null!);
        var existingEntrees = new List<EntreeDto>
        {
            new EntreeDto { Id = 1, EntreeName = "Burger", StationId = 1 }
        };
        var newEntree = new EntreeDto { EntreeName = "Burger", StationId = 1 };

        // Act
        var result = vm.ValidateEntree(existingEntrees, newEntree);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateEntree_ReturnsTrue_WhenNoDuplicateExists()
    {
        // Arrange
        var vm = new CreateOrEditEntreeVM(null!, null!);
        var existingEntrees = new List<EntreeDto>
        {
            new EntreeDto { Id = 1, EntreeName = "Burger", StationId = 1 }
        };
        var newEntree = new EntreeDto { EntreeName = "Pizza", StationId = 1 };

        // Act
        var result = vm.ValidateEntree(existingEntrees, newEntree);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateEntree_ReturnsTrue_WhenDuplicateExistsInDifferentStation()
    {
        // Arrange
        var vm = new CreateOrEditEntreeVM(null!, null!);
        var existingEntrees = new List<EntreeDto>
        {
            new EntreeDto { Id = 1, EntreeName = "Burger", StationId = 1 }
        };
        var newEntree = new EntreeDto { EntreeName = "Burger", StationId = 2 };

        // Act
        var result = vm.ValidateEntree(existingEntrees, newEntree);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateDrink_ReturnsFalse_WhenDuplicateExists()
    {
        // Arrange
        var vm = new CreateOrEditDrinkVM(null!, null!, null!);
        var existingDrinks = new List<DrinkDto>
        {
            new DrinkDto { Id = 1, DrinkName = "Coke", StationId = 1 }
        };
        var newDrink = new DrinkDto { DrinkName = "Coke", StationId = 1 };

        // Act
        var result = vm.ValidateDrink(existingDrinks, newDrink);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSide_ReturnsFalse_WhenDuplicateExists()
    {
        // Arrange
        var vm = new CreateOrEditSideVM(null!, null!);
        var existingSides = new List<SideDto>
        {
            new SideDto { Id = 1, SideName = "Fries", StationId = 1 }
        };
        var newSide = new SideDto { SideName = "Fries", StationId = 1 };

        // Act
        var result = vm.ValidateSide(existingSides, newSide);

        // Assert
        Assert.False(result);
    }
}
