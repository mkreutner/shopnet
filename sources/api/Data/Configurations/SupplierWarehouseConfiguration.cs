using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class SupplierWarehouseConfiguration : IEntityTypeConfiguration<SupplierWarehouse>
{
    public void Configure(EntityTypeBuilder<SupplierWarehouse> builder)
    {
        builder.HasKey(sw => new { sw.SupplierId, sw.WarehouseId });

        builder.HasOne(sw => sw.Supplier)
               .WithMany(s => s.SupplierWarehouses)
               .HasForeignKey(sw => sw.SupplierId)
               .IsRequired(false);

        builder.HasOne(sw => sw.Warehouse)
               .WithMany(w => w.SupplierWarehouses)
               .HasForeignKey(sw => sw.WarehouseId);
    }
}