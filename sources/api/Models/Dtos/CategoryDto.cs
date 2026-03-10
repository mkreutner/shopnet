using System;

namespace ShopNetApi.Models.Dtos;

public record CreateCategoryDto(
    string Name, 
    string? Description, 
    Guid? ParentCategoryId
);

public record UpdateCategoryDto(
    string Name, 
    string? Description, 
    Guid? ParentCategoryId
);

public record CategoryResponseDto(
    Guid Id, 
    string Name, 
    string? Description, 
    Guid? ParentCategoryId,
    string? ParentName,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
);
