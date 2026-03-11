using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopNetApi.Models.Entities;
using ShopNetApi.Controllers;
using ShopNetApi.Data;

namespace ShopNetApi.Controllers.System;

[ApiController]
[Route("[controller]")]
public class SystemController : BaseController
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public SystemController(RoleManager<IdentityRole<Guid>> roleManager, MainDbContext context) : base(context)
    {
        _roleManager = roleManager;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        // On récupère les rôles depuis la base via le RoleManager
        var roles = _roleManager.Roles.Select(r => r.Name).ToList();
        return Ok(new { Count = roles.Count, Roles = roles });
    }
}