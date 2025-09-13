namespace Cafeteria.Shared;

public class IngredientType
{
    public string Name { get; private set; }

    public IngredientType(string name)
    {
        Name = name;
    }
}
