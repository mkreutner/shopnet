using System.ComponentModel.DataAnnotations;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Entities;

public class Brand : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
}