using ErpIntegration.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ErpIntegration.IntegrationTests;

/// <summary>
/// Hosts the real API pipeline but swaps SQLite for an isolated in-memory store,
/// so tests exercise routing, validation, middleware and DI end-to-end without a database.
/// </summary>
public sealed class CustomApiFactory : WebApplicationFactory<Program>
{
    // One stable database name per factory instance. Computed here (not inside the
    // options lambda, which runs per DbContext construction) so every request in a
    // test shares the same in-memory store.
    private readonly string _databaseName = $"erp-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ErpDbContext>) ||
                d.ServiceType == typeof(ErpDbContext)).ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<ErpDbContext>(o => o.UseInMemoryDatabase(_databaseName));
        });
    }
}
