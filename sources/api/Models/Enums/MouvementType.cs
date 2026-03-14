using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Enums;

public enum MovementType
{
    IN,         // Entrée (0)
    OUT,        // Sortie (1)
    ADJUSTMENT  // Ajustement (2)
}