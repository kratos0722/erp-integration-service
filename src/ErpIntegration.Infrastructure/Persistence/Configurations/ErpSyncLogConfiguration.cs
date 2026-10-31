using ErpIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErpIntegration.Infrastructure.Persistence.Configurations;

public sealed class ErpSyncLogConfiguration : IEntityTypeConfiguration<ErpSyncLog>
{
    public void Configure(EntityTypeBuilder<ErpSyncLog> builder)
    {
        builder.ToTable("ErpSyncLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ErpReference).HasMaxLength(100);
        builder.Property(x => x.Message).HasMaxLength(1000);
        builder.Property(x => x.TargetErp).HasConversion<int>();
        builder.Property(x => x.Result).HasConversion<int>();
        builder.HasIndex(x => x.CustomerId);
    }
}
