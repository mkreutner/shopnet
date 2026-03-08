using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Commands;

public static class UserCliCommand
{
    public static async Task Execute(IServiceProvider serviceProvider, string[] args)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        
        var dict = args.Where(a => a.Contains("="))
                       .Select(a => a.Split('=', 2))
                       .ToDictionary(s => s[0].ToLower(), s => s[1]);

        // Ajout d'une commande pour créer/vérifier les rôles
        if (args.Contains("--ensure-roles"))
        {
            var rolesInput = dict.GetValueOrDefault("roles", "");
            var roles = rolesInput.Split(',').Select(r => r.Trim()).Where(r => !string.IsNullOrEmpty(r));
            
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    Console.WriteLine($"✅ Rôle '{roleName}' créé.");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Rôle '{roleName}' existe déjà.");
                }
            }
            return; // On arrête après avoir garanti les rôles
        }

        if (args.Contains("--create-user")) 
        {
            var user = new ApplicationUser {
                UserName = dict.GetValueOrDefault("username", "user"),
                Email = dict.GetValueOrDefault("email"),
                FirstName = dict.GetValueOrDefault("firstname", ""),
                LastName = dict.GetValueOrDefault("lastname", ""),
                EmailConfirmed = true
            };
            
            var password = dict.GetValueOrDefault("password", "ShopNet75!");
            var result = await userManager.CreateAsync(user, password);
            
            if (result.Succeeded) 
            {
                var roleString = dict.GetValueOrDefault("roles"); // Ex: --roles=Administrator,IT
                if (!string.IsNullOrEmpty(roleString))
                {
                    var roles = roleString.Split(',');
                    foreach (var roleName in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName)) 
                            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    }
                    await userManager.AddToRolesAsync(user, roles);
                }
                Console.WriteLine($"✅ Utilisateur {user.Email} créé avec succès.");
            }
            else 
            {
                result.Errors.ToList().ForEach(e => Console.WriteLine($"❌ {e.Description}"));
            }
        }
        else
        {
            Console.WriteLine("❌ Commande inconnue. Utilise : --create-user username=... email=... password=... roles=Role1,Role2");
        }
    }
}