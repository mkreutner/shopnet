using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SuppliersController : BaseController
{
    public SuppliersController(MainDbContext context) : base(context) { }

    // POST: api/suppliers
    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    // PUT: api/suppliers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Supplier supplier)
    {
        if (id != supplier.Id) return BadRequest("ID mismatch");

        // EF Core va automatiquement gérer la mise à jour des entités enfants
        // si elles sont incluses dans le modèle soumis
        _context.Entry(supplier).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Suppliers.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/suppliers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return NotFound();

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent(); // Code 204: Suppression effectuée avec succès
    }

    // GET: api/suppliers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetById(Guid id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Addresses)
            .Include(s => s.Contacts)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return NotFound();

        return Ok(supplier);
    }

    // GET: api/suppliers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
    {
        return await _context.Suppliers
            .Include(s => s.Addresses)
            .Include(s => s.Contacts)
            .ToListAsync();
    }
}