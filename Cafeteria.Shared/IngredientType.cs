namespace Cafeteria.Shared;

public class IngredientType
{
    public string Name { get; private set; }
    public int Quantity { get; set; }

    public IngredientType(string name, int quantity = 1)
    {
        Name = name;
        Quantity = quantity;
    }
}
