namespace Cafeteria.Customer.DTOs;

public class BusinessHoursDto
{
    public string Weekday { get; set; } = string.Empty;
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}