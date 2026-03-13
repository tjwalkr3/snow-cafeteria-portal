using Cafeteria.Customer.Components.Pages.Stations.Domain;

namespace Cafeteria.UnitTests.Customer.Domain;

public class TabDefinitionTests
{
    [Fact]
    public void TabDefinition_DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var tab = new TabDefinition();

        // Assert
        Assert.Equal(string.Empty, tab.Id);
        Assert.Equal(string.Empty, tab.DisplayName);
        Assert.False(tab.IsDefault);
    }

    [Fact]
    public void TabDefinition_ParameterizedConstructor_InitializesCorrectly()
    {
        // Act
        var tab = new TabDefinition("entrees", "Entrees");

        // Assert
        Assert.Equal("entrees", tab.Id);
        Assert.Equal("Entrees", tab.DisplayName);
        Assert.False(tab.IsDefault);
    }

    [Fact]
    public void TabDefinition_ParameterizedConstructorWithDefault_InitializesWithDefaultFlag()
    {
        // Act
        var tab = new TabDefinition("entrees", "Entrees", isDefault: true);

        // Assert
        Assert.Equal("entrees", tab.Id);
        Assert.Equal("Entrees", tab.DisplayName);
        Assert.True(tab.IsDefault);
    }

    [Fact]
    public void TabDefinition_Properties_AreInitOnly()
    {
        // Arrange
        var tab = new TabDefinition("sides", "Sides", true);

        // Act & Assert
        Assert.Equal("sides", tab.Id);
        Assert.Equal("Sides", tab.DisplayName);
        Assert.True(tab.IsDefault);
    }
}
