using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Models.Entities;
using ShopNetApi.Interfaces;
using ShopNetApi.Models.Common;

namespace ShopNetApi.Data;

public class MainDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MainDbContext(DbContextOptions<MainDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) 
    { 
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tva> Tvas => Set<Tva>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierWarehouse> SupplierWarehouses => Set<SupplierWarehouse>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // 1. Appeler la base en PREMIER pour initialiser les conventions Identity
        base.OnModelCreating(builder);

        // 2. Maintenant, appliquer les changements de noms (ils ne seront plus écrasés)
        builder.Entity<ApplicationUser>(entity => entity.ToTable("users"));
        builder.Entity<IdentityRole<Guid>>(entity => entity.ToTable("roles"));
        builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("user_roles"));
        builder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("role_claims"));
        builder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("user_claims"));
        builder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("user_logins"));
        builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("user_tokens"));

        // 3. Appliquer le reste de tes configurations (Fluent API)
        builder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);

        // 4. Correction spécifique pour l'entrepôt
        builder.Entity<Warehouse>()
            .HasOne(w => w.Address)
            .WithMany()
            .HasForeignKey(w => w.AddressId)
            .IsRequired(false); // <--- L'adresse devient optionnelle en BDD
        builder.Entity<StockMovement>()
            .Property(s => s.Type)
            .HasConversion<string>(); // Sauvegarde l'Enum en tant que texte en BDD

        // 4. Boucle pour forcer le snake_case sur le reste
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // On ne renomme que si la table n'a pas encore de nom spécifique
            var tableName = entity.GetTableName();
            if (tableName != null && !tableName.StartsWith("asp_net_")) 
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditable>();
        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added) {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = userId;
            }
            if (entry.State == EntityState.Modified) {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    // Fonction utilitaire pour convertir CamelCase en snake_case
    private string ToSnakeCase(string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }
}