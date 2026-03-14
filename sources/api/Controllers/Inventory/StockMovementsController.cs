using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;
using ShopNetApi.Models.Enums;

namespace ShopNetApi.Controllers.Inventory;

[ApiController]
[Route("[controller]")]
public class StockMovementsController : BaseController
{
    public StockMovementsController(MainDbContext context) : base(context) { }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateMovement(StockMovement movement)
    {
        // 1. Enregistrer le mouvement dans l'historique
        _context.StockMovements.Add(movement);

        // 2. Trouver ou initialiser le stock
        var stock = await _context.ProductStocks
            .FirstOrDefaultAsync(ps => ps.ProductId == movement.ProductId 
                                    && ps.WarehouseId == movement.WarehouseId);

        if (stock == null)
        {
            stock = new ProductStock 
            { 
                ProductId = movement.ProductId, 
                WarehouseId = movement.WarehouseId, 
                Quantity = 0 
            };
            _context.ProductStocks.Add(stock);
        }

        // 3. Appliquer la modification
        stock.Quantity += movement.QuantityChange;

        // 4. Vérification métier : SEULEMENT si c'est une sortie (OUT)
        // On ne bloque pas si c'est une entrée (IN) ou un ajustement technique
        if (movement.Type == MovementType.OUT && stock.Quantity < 0) 
        {
            return BadRequest(new { message = "Stock insuffisant pour cette sortie." });
        }

        await _context.SaveChangesAsync();
        return Ok();
    }
}