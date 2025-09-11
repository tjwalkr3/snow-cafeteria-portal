namespace Cafeteria.Shared;

public class Location
{
    #region Properties
    public string Name { get; init; }
    public string Address { get; init; }
    // TODO: I'm not sure if we actually want these in here...and if we do, maybe there should be a list of DateTimes for the different days of the week
    // DateTime DateTimeOpenMDT { get; set; } 
    // DateTime DateTimeClosedMDT { get; set; }
    #endregion

    #region Constructor
    public Location(string name, string address /*, DateTime open, DateTime closed*/)
    {
        Name = name;
        Address = address;
        // DateTimeOpenMDT = open;
        // DateTimeClosedMDT = closed;
    }
    #endregion
}
