using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Models.Enums;

public enum AddressType
{
    BillingSupplier,   // Facturation fournisseur
    BillingCustomer,   // Facturation client
    ShippingCustomer,  // Livraison client
    Depot              // Dépôt
}