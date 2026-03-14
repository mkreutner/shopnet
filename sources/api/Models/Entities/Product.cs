using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    [ValidateNever]
    [JsonIgnore]
    public Brand? Brand { get; set; }
    
    public Guid CategoryId { get; set; }
    [ValidateNever]
    [JsonIgnore]
    public Category? Category { get; set; }
    
    public Guid SupplierId { get; set; }
    [ValidateNever]
    [JsonIgnore]
    public Supplier? Supplier { get; set; }

    // Navigation
    public ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
}