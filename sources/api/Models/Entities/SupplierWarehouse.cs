using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class SupplierWarehouse
{
    public Guid SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; } = null!;
    
    public Guid WarehouseId { get; set; }
    public virtual Warehouse Warehouse { get; set; } = null!;
    
    // Exemple de donnée métier liée à la relation
    public int LeadTimeDays { get; set; } 
}