namespace Cafeteria.Shared.Extensions;

public static class TimeOnlyExtensions
{
    public static TimeSpan ToTimeSpan(this TimeOnly timeOnly)
    {
        return new TimeSpan(timeOnly.Hour, timeOnly.Minute, timeOnly.Second);
    }
}
