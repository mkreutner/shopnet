using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers.Inventory;

[ApiController]
[Route("[controller]")]
public class WarehousesController : BaseController
{
    public WarehousesController(MainDbContext context) : base(context) { }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll()
    {
        return await _context.Warehouses.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = warehouse.Id }, warehouse);
    }
}