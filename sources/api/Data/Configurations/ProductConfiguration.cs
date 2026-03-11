using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
        builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PurchasePrice).HasPrecision(18, 2);
        builder.Property(p => p.SellingPriceHT).HasPrecision(18, 2);
    }
}