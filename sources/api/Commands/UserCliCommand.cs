using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; // Nécessaire pour .MigrateAsync()
using ShopNetApi.Models.Entities;
using ShopNetApi.Data;

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


        if (args.Contains("--reset-db"))
        {
            var context = serviceProvider.GetRequiredService<MainDbContext>();
            
            // Au lieu de supprimer toute la DATABASE, on nettoie le contenu
            // Cela évite l'erreur de connexion Postgres
            var tableNames = context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Where(name => name != "__EFMigrationsHistory")
                .Distinct()
                .ToList();

            foreach (var tableName in tableNames)
            {
                var sql = string.Format("TRUNCATE TABLE \"{0}\" RESTART IDENTITY CASCADE;", tableName);
    
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            
            Console.WriteLine("✅ L'ensemble des tables réinitialisées avec succés.");
        } 
        else if (args.Contains("--ensure-roles"))
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
        }
        else if (args.Contains("--create-user")) 
        {
            var user = new ApplicationUser {
                UserName = dict.GetValueOrDefault("username", "unknown-user"),
                Email = dict.GetValueOrDefault("email", "no-email@missing.tld"),
                FirstName = dict.GetValueOrDefault("firstname", "Unknown"),
                LastName = dict.GetValueOrDefault("lastname", "User"),
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