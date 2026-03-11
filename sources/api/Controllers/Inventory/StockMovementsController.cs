using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers.Inventory;

[ApiController]
[Route("[controller]")]
public class StockMovementsController : BaseController
{
    public StockMovementsController(MainDbContext context) : base(context) { }

    [HttpPost]
    public async Task<ActionResult> CreateMovement(StockMovement movement)
    {
        // 1. Enregistrer le mouvement dans l'historique
        _context.StockMovements.Add(movement);

        // 2. Trouver le stock actuel du produit dans le dépôt concerné
        var stock = await _context.ProductStocks
            .FirstOrDefaultAsync(ps => ps.ProductId == movement.ProductId 
                                    && ps.WarehouseId == movement.WarehouseId);

        // 3. Si le produit n'existe pas encore dans ce dépôt, on le crée
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

        // 4. Appliquer la modification
        stock.Quantity += movement.QuantityChange;

        // Validation simple : on ne peut pas avoir un stock négatif (optionnel)
        if (stock.Quantity < 0) return BadRequest("Stock insuffisant.");

        await _context.SaveChangesAsync();
        return Ok();
    }
}