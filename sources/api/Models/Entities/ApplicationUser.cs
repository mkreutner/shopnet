using Microsoft.AspNetCore.Identity;
using ShopNetApi.Interfaces;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditable
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Nouveau : Type d'utilisateur pour gérer les profils métiers
    public UserType UserType { get; set; }

    // Navigation vers les profils métiers
    public virtual EmployeeProfile? EmployeeProfile { get; set; }

    // Implémentation de IAuditable
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
}