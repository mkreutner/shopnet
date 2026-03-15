using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNetApi.Models.Entities;

namespace ShopNetApi.Data.Configurations;

public class EmployeeProfileConfiguration : IEntityTypeConfiguration<EmployeeProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeProfile> builder)
    {
        builder.ToTable("employee_profiles");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Service).IsRequired().HasMaxLength(100);
        
        // Relation avec User
        builder.HasOne(e => e.User)
               .WithOne(u => u.EmployeeProfile)
               .HasForeignKey<EmployeeProfile>(e => e.UserId);
    }
}