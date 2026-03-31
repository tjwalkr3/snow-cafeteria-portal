using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class StationDraftToOrderMapperTests
{
    [Fact]
    public void MapCardSelections_MapsQuantitiesAndOptionsIntoOrderItems()
    {
        // Arrange
        var entreeOptionType = new FoodOptionTypeDto { Id = 100, FoodOptionTypeName = "Cheese", MaxAmount = 1 };
        var sideOptionType = new FoodOptionTypeDto { Id = 200, FoodOptionTypeName = "Sauce", MaxAmount = 3 };

        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger", EntreePrice = 5.99m }
        };
        var sides = new List<SideWithOptionsDto>
        {
            new()
            {
                Side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 1.99m },
                OptionTypes =
                [
                    new FoodOptionTypeWithOptionsDto
                    {
                        OptionType = sideOptionType,
                        Options =
                        [
                            new FoodOptionDto { Id = 201, FoodOptionName = "Ketchup" },
                            new FoodOptionDto { Id = 202, FoodOptionName = "Ranch" }
                        ]
                    }
                ]
            }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "Soda", DrinkPrice = 1.49m }
        };

        var entreeQuantities = new Dictionary<int, int> { [1] = 2 };
        var sideQuantities = new Dictionary<int, int> { [2] = 1 };
        var drinkQuantities = new Dictionary<int, int> { [3] = 3 };

        var entreeOptionTypes = new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>
        {
            [1] =
            [
                new FoodOptionTypeWithOptionsDto
                {
                    OptionType = entreeOptionType,
                    Options =
                    [
                        new FoodOptionDto { Id = 101, FoodOptionName = "American" },
                        new FoodOptionDto { Id = 102, FoodOptionName = "Cheddar" }
                    ]
                }
            ]
        };
        var sideOptionTypes = new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>
        {
            [2] = sides[0].OptionTypes
        };
        var entreeOptions = new Dictionary<int, Dictionary<int, HashSet<string>>>
        {
            [1] = new Dictionary<int, HashSet<string>>
            {
                [100] = ["Cheddar"]
            }
        };
        var sideOptions = new Dictionary<int, Dictionary<int, HashSet<string>>>
        {
            [2] = new Dictionary<int, HashSet<string>>
            {
                [200] = ["Ketchup"]
            }
        };

        // Act
        var result = StationDraftToOrderMapper.MapCardSelections(new CardStationDraft
        {
            Entrees = entrees,
            Sides = sides,
            Drinks = drinks,
            EntreeQuantities = entreeQuantities,
            SideQuantities = sideQuantities,
            DrinkQuantities = drinkQuantities,
            EntreeOptionTypes = entreeOptionTypes,
            SideOptionTypes = sideOptionTypes,
            EntreeOptions = entreeOptions,
            SideOptions = sideOptions
        });

        // Assert
        Assert.Equal(2, result.Entrees.Count);
        Assert.All(result.Entrees, item => Assert.Equal("Burger", item.Entree.EntreeName));
        Assert.All(result.Entrees, item => Assert.Single(item.SelectedOptions));
        Assert.All(result.Entrees, item => Assert.Equal("Cheddar", item.SelectedOptions[0].Option.FoodOptionName));

        Assert.Single(result.Sides);
        Assert.Equal("Fries", result.Sides[0].Side.SideName);
        Assert.Single(result.Sides[0].SelectedOptions);
        Assert.Equal("Ketchup", result.Sides[0].SelectedOptions[0].Option.FoodOptionName);

        Assert.Equal(3, result.Drinks.Count);
        Assert.All(result.Drinks, drink => Assert.Equal("Soda", drink.DrinkName));
    }

    [Fact]
    public void MapCardSelections_UsesUniformOptionShapes_ForEntreesAndSides()
    {
        // Arrange
        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger", EntreePrice = 5.99m }
        };

        var sides = new List<SideWithOptionsDto>
        {
            new()
            {
                Side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 1.99m },
                OptionTypes =
                [
                    new FoodOptionTypeWithOptionsDto
                    {
                        OptionType = new FoodOptionTypeDto { Id = 200, FoodOptionTypeName = "Sauce", MaxAmount = 3 },
                        Options =
                        [
                            new FoodOptionDto { Id = 201, FoodOptionName = "Ketchup" },
                            new FoodOptionDto { Id = 202, FoodOptionName = "Ranch" }
                        ]
                    }
                ]
            }
        };

        var entreeOptionType = new FoodOptionTypeDto { Id = 100, FoodOptionTypeName = "Cheese", MaxAmount = 1 };
        var entreeOptionTypes = new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>
        {
            [1] =
            [
                new FoodOptionTypeWithOptionsDto
                {
                    OptionType = entreeOptionType,
                    Options =
                    [
                        new FoodOptionDto { Id = 101, FoodOptionName = "American" },
                        new FoodOptionDto { Id = 102, FoodOptionName = "Cheddar" }
                    ]
                }
            ]
        };

        var sideOptionTypes = new Dictionary<int, List<FoodOptionTypeWithOptionsDto>>
        {
            [2] = sides[0].OptionTypes
        };

        var draft = new CardStationDraft
        {
            Entrees = entrees,
            Sides = sides,
            EntreeQuantities = new Dictionary<int, int> { [1] = 1 },
            SideQuantities = new Dictionary<int, int> { [2] = 1 },
            EntreeOptionTypes = entreeOptionTypes,
            SideOptionTypes = sideOptionTypes,
            EntreeOptions = new Dictionary<int, Dictionary<int, HashSet<string>>>
            {
                [1] = new Dictionary<int, HashSet<string>>
                {
                    [100] = ["Cheddar"]
                }
            },
            SideOptions = new Dictionary<int, Dictionary<int, HashSet<string>>>
            {
                [2] = new Dictionary<int, HashSet<string>>
                {
                    [200] = ["Ranch"]
                }
            }
        };

        // Act
        var result = StationDraftToOrderMapper.MapCardSelections(draft);

        // Assert
        Assert.Single(result.Entrees);
        Assert.Single(result.Entrees[0].SelectedOptions);
        Assert.Equal("Cheddar", result.Entrees[0].SelectedOptions[0].Option.FoodOptionName);

        Assert.Single(result.Sides);
        Assert.Single(result.Sides[0].SelectedOptions);
        Assert.Equal("Ranch", result.Sides[0].SelectedOptions[0].Option.FoodOptionName);
    }

    [Fact]
    public void MapCardSelections_SkipsUnknownItemsAndNonPositiveQuantities()
    {
        // Arrange
        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger" }
        };
        var sides = new List<SideWithOptionsDto>
        {
            new() { Side = new SideDto { Id = 2, SideName = "Fries" } }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "Soda" }
        };

        var entreeQuantities = new Dictionary<int, int> { [1] = 0, [999] = 2 };
        var sideQuantities = new Dictionary<int, int> { [2] = -1 };
        var drinkQuantities = new Dictionary<int, int> { [999] = 1 };

        // Act
        var result = StationDraftToOrderMapper.MapCardSelections(new CardStationDraft
        {
            Entrees = entrees,
            Sides = sides,
            Drinks = drinks,
            EntreeQuantities = entreeQuantities,
            SideQuantities = sideQuantities,
            DrinkQuantities = drinkQuantities
        });

        // Assert
        Assert.Empty(result.Entrees);
        Assert.Empty(result.Sides);
        Assert.Empty(result.Drinks);
    }

    [Fact]
    public void MapCardSelections_ReturnsEmptyWhenNoSelections()
    {
        // Arrange
        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger" }
        };
        var sides = new List<SideWithOptionsDto>
        {
            new() { Side = new SideDto { Id = 2, SideName = "Fries" } }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "Soda" }
        };

        // Act
        var result = StationDraftToOrderMapper.MapCardSelections(new CardStationDraft
        {
            Entrees = entrees,
            Sides = sides,
            Drinks = drinks
        });

        // Assert
        Assert.Empty(result.Entrees);
        Assert.Empty(result.Sides);
        Assert.Empty(result.Drinks);
    }

    [Fact]
    public void MapCardSelections_UsesSideFallbackOptionTypes_WhenSideOptionTypesMapMissing()
    {
        // Arrange
        var sides = new List<SideWithOptionsDto>
        {
            new()
            {
                Side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 1.99m },
                OptionTypes =
                [
                    new FoodOptionTypeWithOptionsDto
                    {
                        OptionType = new FoodOptionTypeDto { Id = 200, FoodOptionTypeName = "Sauce", MaxAmount = 3 },
                        Options =
                        [
                            new FoodOptionDto { Id = 201, FoodOptionName = "Ketchup" }
                        ]
                    }
                ]
            }
        };

        var result = StationDraftToOrderMapper.MapCardSelections(new CardStationDraft
        {
            Sides = sides,
            SideQuantities = new Dictionary<int, int> { [2] = 1 },
            SideOptions = new Dictionary<int, Dictionary<int, HashSet<string>>>
            {
                [2] = new Dictionary<int, HashSet<string>>
                {
                    [200] = ["Ketchup"]
                }
            }
        });

        // Assert
        Assert.Single(result.Sides);
        Assert.Single(result.Sides[0].SelectedOptions);
        Assert.Equal("Ketchup", result.Sides[0].SelectedOptions[0].Option.FoodOptionName);
    }
}