using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.HasKey(ps => ps.Id);
        
        // Index unique pour éviter les doublons Produit/Dépôt
        builder.HasIndex(ps => new { ps.ProductId, ps.WarehouseId }).IsUnique();
        
        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.Stocks)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}