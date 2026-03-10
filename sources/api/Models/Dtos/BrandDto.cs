using System;

namespace ShopNetApi.Models.Dtos;

public record CreateBrandDto(string Name, string? Description);

public record UpdateBrandDto(string Name, string? Description);

public record BrandResponseDto(
    Guid Id, 
    string Name, 
    string? Description, 
    DateTime CreatedAt, 
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
);