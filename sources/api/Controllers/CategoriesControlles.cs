using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Dtos;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly MainDbContext _context;

    public CategoriesController(MainDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
    {
        var categories = await _context.Categories
            .Include(c => c.ParentCategory)
            .ToListAsync();

        return Ok(categories.Select(c => new CategoryResponseDto(
            c.Id, 
            c.Name, 
            c.Description, 
            c.ParentCategoryId, 
            c.ParentCategory?.Name,
            c.CreatedAt, 
            c.CreatedBy, 
            c.UpdatedAt, 
            c.UpdatedBy)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetCategory(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        return Ok(new CategoryResponseDto(
            category.Id, 
            category.Name, 
            category.Description, 
            category.ParentCategoryId, 
            category.ParentCategory?.Name,
            category.CreatedAt, 
            category.CreatedBy, 
            category.UpdatedAt, 
            category.UpdatedBy));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CreateCategoryDto dto)
    {
        // Vérifier si le parent existe si un ID est fourni
        if (dto.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.Categories.AnyAsync(c => c.Id == dto.ParentCategoryId);
            if (!parentExists) return BadRequest("Parent category not found.");
        }

        var category = new Category
        {
            Id = Guid.CreateVersion7(),
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        // On recharge l'entité avec son Parent pour avoir le nom
        var createdCategory = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        // Retourne le DTO complet et hydraté
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory!.Id }, 
            new CategoryResponseDto(
                createdCategory.Id, 
                createdCategory.Name, 
                createdCategory.Description, 
                createdCategory.ParentCategoryId, 
                createdCategory.ParentCategory?.Name, // Le nom est maintenant disponible !
                createdCategory.CreatedAt, 
                createdCategory.CreatedBy, 
                createdCategory.UpdatedAt, 
                createdCategory.UpdatedBy
            ));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        // Empêcher de devenir son propre parent
        if (dto.ParentCategoryId == id) return BadRequest("A category cannot be its own parent.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ParentCategoryId = dto.ParentCategoryId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        if (category.SubCategories.Any())
            return BadRequest("Cannot delete category with sub-categories. Delete children first.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}