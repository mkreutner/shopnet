using System;

namespace ShopNetApi.Models.Dtos;

public class ReassignDto
{
    public Guid OldManagerId { get; set; }
    public Guid NewManagerId { get; set; }
}