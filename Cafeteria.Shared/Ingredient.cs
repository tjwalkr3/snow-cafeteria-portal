namespace Cafeteria.Shared;

public class Ingredient
{
    #region Properties
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    #endregion

    #region Constructor
    public Ingredient(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
    #endregion
}
