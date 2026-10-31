using ErpIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpIntegration.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.CustomerErpId).HasMaxLength(100);
        builder.Property(x => x.ErpInvoiceId).HasMaxLength(100);
        builder.Property(x => x.TargetErp).HasConversion<int>();
        builder.Property(x => x.SyncStatus).HasConversion<int>();
        builder.HasIndex(x => x.CustomerId);
    }
}
