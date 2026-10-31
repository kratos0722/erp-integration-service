using ErpIntegration.Application.Customers;
using ErpIntegration.Application.Invoices;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ErpIntegration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();
        return services;
    }
}
