using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers.Catalog;

[ApiController]
[Route("[controller]")]
public class ProductsController : BaseController
{
    public ProductsController(MainDbContext context) : base(context) { }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Stocks)
            .ToListAsync();
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        // Logique métier : Calcul auto du prix de vente HT si non spécifié
        if (product.SellingPriceHT == 0 && product.PurchasePrice > 0)
        {
            product.SellingPriceHT = product.PurchasePrice * (1 + (product.TargetMarginPercent / 100));
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Product product)
    {
        if (id != product.Id) return BadRequest("ID mismatch");

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}