using System;

namespace ShopNetApi.Models.Dtos;

public class EmployeeUpdateDto
{
    public string? Service { get; set; }
    public Guid? ManagerId { get; set; } // Nullable pour permettre le "null" ou l'absence de valeur
}