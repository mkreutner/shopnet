using System;

namespace ShopNetApi.Models.Dtos;

public class EmployeeCreationDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
}