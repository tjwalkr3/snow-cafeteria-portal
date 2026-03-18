namespace Cafeteria.Shared.DTOs.Customer;

public class CustomerRoleDto
{
    public string Email { get; set; } = string.Empty;
    public string CustName { get; set; } = string.Empty;
    public int BadgerId { get; set; }
    public string? UserRole { get; set; }
}
