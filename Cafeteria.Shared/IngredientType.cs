namespace Cafeteria.Shared;

public class IngredientType
{
    public string Name { get; private set; }
    public int Quantity { get; set; }

    public IngredientType(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }
}
