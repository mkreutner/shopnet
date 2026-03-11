using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Models.Entities;

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int QuantityChange { get; set; } // Négatif pour sortie, positif pour entrée
    public string MovementType { get; set; } = string.Empty; // "IN", "OUT", "ADJUSTMENT"
    public string Reason { get; set; } = string.Empty;
}