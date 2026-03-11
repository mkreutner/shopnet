using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(sm => sm.Id);

        // Définition des relations
        builder.HasOne(sm => sm.Product)
            .WithMany()
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // On empêche la suppression d'un produit s'il a des mouvements

        builder.HasOne(sm => sm.Warehouse)
            .WithMany()
            .HasForeignKey(sm => sm.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sm => sm.MovementType).IsRequired().HasMaxLength(20);
        builder.Property(sm => sm.QuantityChange).IsRequired();
    }
}
