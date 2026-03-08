using ShopNetApi.Interfaces;

namespace ShopNetApi.Models.Common;

public abstract class BaseEntity : IAuditable
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}