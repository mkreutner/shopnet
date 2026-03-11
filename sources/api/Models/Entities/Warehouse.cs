using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    
    // Une adresse principale pour le dépôt
    [ForeignKey(nameof(Address))]
    public Guid AddressId { get; set; }
    public virtual Address Address { get; set; } = null!;

    // Relation N-N
    public virtual ICollection<SupplierWarehouse> SupplierWarehouses { get; set; } = new List<SupplierWarehouse>();
}