using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers.Inventory;

[ApiController]
[Route("[controller]")]
public class InventoryStatusController : BaseController
{
    public InventoryStatusController(MainDbContext context) : base(context) { }

    // GET: api/InventoryStatus/{productId}
    // Permet de voir le stock total d'un produit sur tous les entrepôts
    [HttpGet("{productId}")]
    public async Task<ActionResult<int>> GetTotalStock(Guid productId)
    {
        var totalStock = await _context.ProductStocks
            .Where(ps => ps.ProductId == productId)
            .SumAsync(ps => ps.Quantity);

        return Ok(totalStock);
    }

    // GET: api/InventoryStatus
    // Permet de voir le détail du stock par entrepôt pour un produit donné
    [HttpGet("details/{productId}")]
    public async Task<ActionResult<IEnumerable<ProductStock>>> GetDetails(Guid productId)
    {
        return await _context.ProductStocks
            .Include(ps => ps.Warehouse)
            .Where(ps => ps.ProductId == productId)
            .ToListAsync();
    }
}