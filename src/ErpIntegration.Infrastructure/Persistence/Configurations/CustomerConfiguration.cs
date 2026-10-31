using ErpIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpIntegration.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Email);
        builder.Property(x => x.Phone).HasMaxLength(40);
        builder.Property(x => x.ErpCustomerId).HasMaxLength(100);
        builder.Property(x => x.TargetErp).HasConversion<int>();
        builder.Property(x => x.SyncStatus).HasConversion<int>();
    }
}
