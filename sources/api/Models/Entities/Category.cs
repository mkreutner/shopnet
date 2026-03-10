using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Entities;

public class Category : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Parent ID
    public Guid? ParentCategoryId { get; set; }
    
    [ForeignKey("ParentCategoryId")]
    public virtual Category? ParentCategory { get; set; }

    // Children
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
}