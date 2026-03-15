using System;
using ShopNetApi.Interfaces;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Entities;

public class EmployeeProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string Service { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    
    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual EmployeeProfile? Manager { get; set; }
}