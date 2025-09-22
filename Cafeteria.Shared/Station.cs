using System.ComponentModel;

namespace Cafeteria.Shared;

public class Station
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Station(string name, string description)
    {
        Name = name;
        Description = description;
    }

}
