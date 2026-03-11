using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    
    // Relations
    public virtual ICollection<SupplierWarehouse> SupplierWarehouses { get; set; } = new List<SupplierWarehouse>();
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}