using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Entities;

public class Tva : BaseEntity
{
    public string Name { get; set; } = string.Empty;    // ex: "TVA 20%"
    public decimal Rate { get; set; }                   // ex: 0.20
}