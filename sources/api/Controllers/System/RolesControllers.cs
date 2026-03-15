using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopNetApi.Data;
using ShopNetApi.Controllers;

namespace ShopNetApi.Controllers.System;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Administrator")]
public class RolesController : BaseController
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RolesController(RoleManager<IdentityRole<Guid>> roleManager, MainDbContext context) : base(context)
    {
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
        return Ok(roles);
    }
}