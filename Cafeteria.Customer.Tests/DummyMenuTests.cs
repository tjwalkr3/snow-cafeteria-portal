using Cafeteria.Customer.Tests.DummyServices;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Tests;

public class DummyMenuTests
{
    private DummyMenuService MenuService;

    public DummyMenuTests()
    {
        MenuService = new DummyMenuService();
    }

    [Fact]
    public async Task DummyMenuServiceReturnsDummyLocationList()
    {
        var locations = await MenuService.GetAllLocations();
        List<LocationDto> locationList = locations.ToList();
        Assert.True((locations is not null) && (locationList.Count > 0));
    }

    [Fact]
    public async Task DummyMenuServiceGetsStationsForDummyLocation()
    {
        var stations = await MenuService.GetStationsByLocation(1);
        List<StationDto> stationList = stations.ToList();
        Assert.True((stations is not null) && (stationList.Count > 0));
    }
}
