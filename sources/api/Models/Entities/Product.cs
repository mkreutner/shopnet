using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    
    // Prix et Marge
    public decimal PurchasePrice { get; set; }
    public decimal TargetMarginPercent { get; set; }
    public decimal SellingPriceHT { get; set; }

    // Garantie (Hiérarchie de priorité)
    public int WarrantyMonths { get; set; } 
    
    // Relations
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    // Navigation
    public ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
}