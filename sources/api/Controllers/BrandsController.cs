using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Dtos;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BrandsController : ControllerBase
{
    private readonly MainDbContext _context;

    public BrandsController(MainDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResponseDto>>> GetBrands()
    {
        var brands = await _context.Brands.ToListAsync();
        
        return Ok(brands.Select(b => new BrandResponseDto(
            b.Id, b.Name, b.Description, b.CreatedAt, b.CreatedBy, b.UpdatedAt, b.UpdatedBy)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandResponseDto>> GetBrand(Guid id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();

        return Ok(new BrandResponseDto(
            brand.Id, brand.Name, brand.Description, brand.CreatedAt, brand.CreatedBy, brand.UpdatedAt, brand.UpdatedBy));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<BrandResponseDto>> CreateBrand(CreateBrandDto dto)
    {
        var brand = new Brand 
        { 
            Id = Guid.CreateVersion7(),
            Name = dto.Name, 
            Description = dto.Description 
        };

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();

        var response = new BrandResponseDto(
            brand.Id, brand.Name, brand.Description, brand.CreatedAt, brand.CreatedBy, brand.UpdatedAt, brand.UpdatedBy);

        return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, response);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBrand(Guid id, UpdateBrandDto dto)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();

        brand.Name = dto.Name;
        brand.Description = dto.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrand(Guid id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}