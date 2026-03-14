using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Product? Product { get; set; }

    public Guid WarehouseId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Warehouse? Warehouse { get; set; }

    public int QuantityChange { get; set; } // Négatif pour sortie, positif pour entrée
    [JsonPropertyName("movement")]
    public MovementType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
}