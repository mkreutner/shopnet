using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopNetApi.Models.Entities;
using ShopNetApi.Models.Common;
using ShopNetApi.Interfaces;

namespace ShopNetApi.Data;

public class MainDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MainDbContext(DbContextOptions<MainDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options) { 
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(entity => entity.ToTable("users"));
        builder.Entity<IdentityRole<Guid>>(entity => entity.ToTable("roles"));
        // Ajout pour renommer les tables de jointure
        builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("user_roles"));
        builder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("role_claims"));
        builder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("user_claims"));
        builder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("user_logins"));
        builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("user_tokens"));
    }
}