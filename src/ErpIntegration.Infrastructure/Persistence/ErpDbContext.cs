using ErpIntegration.Application.Abstractions;
using ErpIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpIntegration.Infrastructure.Persistence;

/// <summary>EF Core context. Doubles as the unit of work for the request scope.</summary>
public sealed class ErpDbContext : DbContext, IUnitOfWork
{
    public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ErpSyncLog> SyncLogs => Set<ErpSyncLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ErpDbContext).Assembly);
    }
}
