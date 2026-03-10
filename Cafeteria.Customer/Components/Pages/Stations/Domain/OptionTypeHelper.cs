using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.Stations.Domain;

public static class OptionTypeHelper
{
    public static bool IsMultiSelectOptionType(FoodOptionTypeWithOptionsDto optionType) =>
        optionType.OptionType.MaxAmount > 1 ||
        optionType.OptionType.FoodOptionTypeName == "Toppings" ||
        optionType.OptionType.FoodOptionTypeName == "Fillings";
}
