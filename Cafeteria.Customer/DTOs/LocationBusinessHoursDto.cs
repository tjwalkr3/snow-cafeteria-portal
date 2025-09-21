namespace Cafeteria.Customer.DTOs;

public class LocationBusinessHoursDto
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public int WeekdayId { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}