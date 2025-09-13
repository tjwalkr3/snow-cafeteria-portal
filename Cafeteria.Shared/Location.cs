namespace Cafeteria.Shared;

public class Location
{
    #region Properties
    public string Name { get; init; }
    public string Address { get; init; }
    #endregion

    #region Constructor
    public Location(string name, string address)
    {
        Name = name;
        Address = address;
    }
    #endregion
}
