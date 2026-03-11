using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Entities;

public class Contact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // ex: "Responsable SAV"

    // Lien vers le fournisseur
    public Guid SupplierId { get; set; }
}