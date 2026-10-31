using ErpIntegration.Application.Abstractions;
using ErpIntegration.Infrastructure.Erp;
using ErpIntegration.Infrastructure.Persistence;
using ErpIntegration.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErpIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=erp.db";
        services.AddDbContext<ErpDbContext>(options => options.UseSqlite(connectionString));

        // The DbContext acts as the unit of work for the scope.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ErpDbContext>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // ERP connectors — registered as a set so the factory can resolve by ERP.
        services.AddScoped<IErpConnector, Sage300Connector>();
        services.AddScoped<IErpConnector, AcumaticaConnector>();
        services.AddScoped<IErpConnectorFactory, ErpConnectorFactory>();

        return services;
    }
}
