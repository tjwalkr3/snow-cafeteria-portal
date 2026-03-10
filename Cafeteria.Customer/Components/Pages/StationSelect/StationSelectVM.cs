using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public class StationSelectVM : IStationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    private Dictionary<int, List<StationBusinessHoursDto>> _businessHoursCache = new();
    private Dictionary<int, List<StationExceptionHoursDto>> _exceptionsCache = new();
    public bool IsInitialized { get; private set; } = false;
    public List<StationDto>? Stations { get; private set; }

    public StationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
        Stations = new List<StationDto>();
    }

    public async Task InitializeStations(int locationId)
    {
        try
        {
            var allStations = await _menuService.GetStationsByLocation(locationId);
            var openStations = new List<StationDto>();

            foreach (var station in allStations)
            {
                // Fetch business hours and exceptions for this station
                var businessHours = await _menuService.GetStationBusinessHours(station.Id);
                var exceptions = await _menuService.GetStationExceptions(station.Id);
                
                _businessHoursCache[station.Id] = businessHours;
                _exceptionsCache[station.Id] = exceptions;

                // Only include the station if it's currently open and has no active exceptions
                if (await IsStationOpenNow(station.Id))
                {
                    openStations.Add(station);
                }
            }

            Stations = openStations;
        }
        catch
        {
            urlParsingFailed = true;
        }
    }

    public async Task<bool> IsStationOpenNow(int stationId)
    {
        // Check if station has any active exceptions
        if (_exceptionsCache.TryGetValue(stationId, out var exceptions))
        {
            var now = DateTime.Now;
            var hasActiveException = exceptions.Any(e => 
                now >= e.StartExceptionDateTime && 
                now <= e.EndExceptionDateTime);
            
            if (hasActiveException)
                return false;
        }

        // Check business hours
        if (_businessHoursCache.TryGetValue(stationId, out var businessHours))
        {
            var now = DateTime.Now;
            var currentWeekday = (int)now.DayOfWeek;
            var todayHours = businessHours.FirstOrDefault(h => h.WeekdayId == currentWeekday);
            
            if (todayHours == null)
                return false; // No hours defined for today

            return now.TimeOfDay >= todayHours.OpenTime.ToTimeSpan() &&
                   now.TimeOfDay <= todayHours.CloseTime.ToTimeSpan();
        }

        return false; // No business hours found
    }

    public bool ErrorOccurredWhileParsingSelectedLocation()
    {
        return urlParsingFailed;
    }
}