using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasMany(s => s.Contacts)
            .WithOne() // Pas de propriété de navigation inverse
            .HasForeignKey(c => c.SupplierId) // Indispensable pour lier la FK
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Addresses)
            .WithOne() // Idem pour les adresses
            .HasForeignKey("SupplierId") // EF va créer une shadow property si elle n'existe pas dans Address
            .OnDelete(DeleteBehavior.Cascade);
    }
}