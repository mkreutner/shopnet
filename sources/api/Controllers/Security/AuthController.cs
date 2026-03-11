using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopNetApi.Models.Entities;
using ShopNetApi.Services;
using ShopNetApi.Controllers;
using ShopNetApi.Data;

namespace ShopNetApi.Controllers.Security;

[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, MainDbContext context) : base(context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // 1. Chercher l'utilisateur
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user == null) return Unauthorized("Utilisateur ou mot de passe incorrect.");

        // 2. Vérifier le mot de passe
        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized("Utilisateur ou mot de passe incorrect.");

        // 3. Récupérer les rôles pour les mettre dans le token
        var roles = await _userManager.GetRolesAsync(user);

        // 4. Générer le token
        return Ok(new
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user, roles)
        });
    }
}

// Petit DTO pour la requête
public record LoginDto(string Username, string Password);