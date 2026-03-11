using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Dtos;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly MainDbContext _context;

    protected BaseController(MainDbContext context)
    {
        _context = context;
    }

    // Tu peux ajouter ici des méthodes helpers comme :
    // protected ActionResult HandleResult<T>(T result) => Ok(result);
}