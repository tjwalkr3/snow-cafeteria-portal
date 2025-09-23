using System.ComponentModel;

namespace Cafeteria.Shared;

public class Location
{
    #region Properties
    public string Name { get; init; }
    public string Address { get; init; }
    public string Description { get; private set; }
    #endregion

    #region Constructor
    public Location(string name, string address)
    {
        Name = name;
        Address = address;
        Description = string.Empty;
    }
    #endregion
}
