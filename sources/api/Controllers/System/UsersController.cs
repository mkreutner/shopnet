using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Data;
using ShopNetApi.Models.Entities;
using ShopNetApi.Models.Dtos;
using ShopNetApi.Models.Enums;
using ShopNetApi.Controllers;

namespace ShopNetApi.Controllers.System;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Administrator,IT")]
public class UsersController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(MainDbContext context, UserManager<ApplicationUser> userManager) : base(context)
    {
        _userManager = userManager;
    }

    [HttpGet("employees/{id}")]
    public async Task<IActionResult> GetEmployee(Guid id)
    {
        // On charge le manager, puis son utilisateur lié, pour éviter de manipuler des ProfileId
        var profile = await _context.EmployeeProfiles
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .FirstOrDefaultAsync(e => e.UserId == id);

        if (profile == null) return NotFound("Profil introuvable.");

        return Ok(new { 
            userId = profile.UserId, 
            // Le ?. gère le cas où Manager ou User sont nuls
            managerId = profile.Manager?.User?.Id, 
            service = profile.Service 
        });
    }

    [HttpPost("employees")]
    public async Task<ActionResult> CreateEmployee([FromBody] EmployeeCreationDto dto)
    {
        // 1. Création de l'utilisateur Identity
        var user = new ApplicationUser 
        { 
            UserName = dto.Email, 
            Email = dto.Email, 
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserType = UserType.EMPLOYEE,
            EmailConfirmed = true // Optionnel : évite de bloquer le login
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // 2. Traduction du ManagerId (UserId -> ProfileId)
        Guid? actualProfileManagerId = null;

        if (dto.ManagerId.HasValue && dto.ManagerId.Value != Guid.Empty)
        {
            // On cherche le PROFIL qui appartient à l'utilisateur envoyé comme manager
            var managerProfile = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(p => p.UserId == dto.ManagerId.Value);

            if (managerProfile == null)
            {
                // Optionnel : supprimer le compte user créé si le manager est invalide
                await _userManager.DeleteAsync(user);
                return BadRequest(new { Error = "Le manager spécifié n'a pas de profil employé valide." });
            }
            
            actualProfileManagerId = managerProfile.Id;
        }

        // 3. Création du profil avec l'ID correct
        var profile = new EmployeeProfile 
        { 
            Id = Guid.NewGuid(), // Bonne pratique de le générer explicitement
            UserId = user.Id, 
            Service = dto.Service,
            ManagerId = actualProfileManagerId // On utilise le ProfileId trouvé
        };
        
        _context.EmployeeProfiles.Add(profile);
        
        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Nettoyage de l'utilisateur si le profil échoue
            await _userManager.DeleteAsync(user);
            throw;
        }

        return Ok(new { UserId = user.Id });
    }

    [HttpDelete("employees/{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        // 1. Récupération avec jointure pour vérifier le type
        var profile = await _context.EmployeeProfiles
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.UserId == id);

        if (profile == null) return NotFound("Profil employé introuvable.");

        // 2. Double vérification de sécurité métier
        if (profile.User.UserType != UserType.EMPLOYEE)
        {
            return BadRequest("Ce compte n'est pas un compte employé.");
        }

        // 3. Soft Delete des deux entités
        _context.EmployeeProfiles.Remove(profile);
        _context.Users.Remove(profile.User);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("employees/{id}")]
    [Authorize(Roles = "Administrator,IT")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeUpdateDto dto)
    {
        // 1. On récupère le profil de l'employé à modifier (via son UserId)
        var employeeProfile = await _context.EmployeeProfiles
            .FirstOrDefaultAsync(p => p.UserId == id);

        if (employeeProfile == null)
            return NotFound(new { Message = "Profil employé introuvable." });

        // 2. Si on demande un changement de manager
        if (dto.ManagerId.HasValue)
        {
            // Traduction : On cherche le PROFILE ID du nouveau manager à partir de son USER ID
            var newManagerProfile = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(p => p.UserId == dto.ManagerId.Value);

            if (newManagerProfile == null)
                return BadRequest(new { Message = "Le nouveau manager spécifié n'a pas de profil employé valide." });

            employeeProfile.ManagerId = newManagerProfile.Id;
        }
        else if (dto.ManagerId == null) 
        {
            // Cas d'une promotion ou d'un détachement (ManagerId mis à null explicitement)
            employeeProfile.ManagerId = null;
        }

        // 3. Autres champs optionnels (ex: Service)
        if (!string.IsNullOrEmpty(dto.Service))
            employeeProfile.Service = dto.Service;

        await _context.SaveChangesAsync();

        return Ok(new { 
            Message = "Employé mis à jour avec succès.",
            EmployeeId = id,
            NewManagerId = dto.ManagerId 
        });
    }
}