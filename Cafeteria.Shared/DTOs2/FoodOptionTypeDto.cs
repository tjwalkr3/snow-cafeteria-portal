using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class FoodOptionTypeDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FoodOptionTypeName { get; set; } = string.Empty;

    [Required]
    [Range(0, short.MaxValue)]
    public short NumIncluded { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MaxAmount { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal FoodOptionPrice { get; set; }

    public int? EntreeId { get; set; }

    public int? SideId { get; set; }
}