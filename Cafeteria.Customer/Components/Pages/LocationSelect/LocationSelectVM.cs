using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public class LocationSelectVM : ILocationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool initializationFailed = false;
    private Dictionary<int, List<LocationBusinessHoursDto>> _businessHoursCache = new();
    private Dictionary<int, List<LocationExceptionHoursDto>> _exceptionsCache = new();
    
    public List<LocationDto> Locations { get; private set; } = new();

    public LocationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task InitializeLocationsAsync()
    {
        try
        {
            var allLocations = await _menuService.GetAllLocations();
            var openLocations = new List<LocationDto>();

            foreach (var location in allLocations)
            {
                // Fetch business hours and exceptions for this location
                var businessHours = await _menuService.GetLocationBusinessHours(location.Id);
                var exceptions = await _menuService.GetLocationExceptions(location.Id);
                
                _businessHoursCache[location.Id] = businessHours;
                _exceptionsCache[location.Id] = exceptions;

                // Only include the location if it's currently open and has no active exceptions
                if (await IsLocationOpenNow(location.Id))
                {
                    openLocations.Add(location);
                }
            }

            Locations = openLocations;
        }
        catch
        {
            initializationFailed = true;
        }
    }

    public async Task<bool> IsLocationOpenNow(int locationId)
    {
        // Check if location has any active exceptions
        if (_exceptionsCache.TryGetValue(locationId, out var exceptions))
        {
            var now = DateTime.Now;
            var hasActiveException = exceptions.Any(e => 
                now >= e.StartExceptionDateTime && 
                now <= e.EndExceptionDateTime);
            
            if (hasActiveException)
                return false;
        }

        // Check business hours
        if (_businessHoursCache.TryGetValue(locationId, out var businessHours))
        {
            var now = DateTime.Now;
            var currentWeekday = (int)now.DayOfWeek;
            // Convert .NET DayOfWeek (0 = Sunday) to WeekDay enum if needed
            var todayHours = businessHours.FirstOrDefault(h => h.WeekdayId == currentWeekday);
            
            if (todayHours == null)
                return false; // No hours defined for today

            return now.TimeOfDay >= todayHours.OpenTime.ToTimeSpan() &&
                   now.TimeOfDay <= todayHours.CloseTime.ToTimeSpan();
        }

        return false; // No business hours found
    }

    public bool ErrorOccurred()
    {
        return Locations == null || initializationFailed;
    }
}

