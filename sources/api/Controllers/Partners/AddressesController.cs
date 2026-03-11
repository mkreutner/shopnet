using Microsoft.AspNetCore.Mvc;
using ShopNetApi.Models.Entities;
using ShopNetApi.Data;

namespace ShopNetApi.Controllers.Partners;

public class AddressesController : BaseController
{
    public AddressesController(MainDbContext context) : base(context) { }

    [HttpPost]
    public async Task<ActionResult<Address>> Create(Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Address>> GetById(Guid id)
    {
        var address = await _context.Addresses.FindAsync(id);
        return address == null ? NotFound() : Ok(address);
    }
}