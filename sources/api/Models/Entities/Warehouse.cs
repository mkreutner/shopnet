using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public WarehouseType Type { get; set; }
    
    // On rend l'AddressId nullable ou on s'assure que le validateur l'ignore
    public Guid? AddressId { get; set; } // Changé en Guid? pour plus de souplesse
    
    [JsonIgnore]
    [ValidateNever]
    public virtual Address? Address { get; set; } // Changé en Address?

    [JsonIgnore]
    [ValidateNever]
    public virtual ICollection<SupplierWarehouse> SupplierWarehouses { get; set; } = new List<SupplierWarehouse>();
}