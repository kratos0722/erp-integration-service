using ErpIntegration.Application.Customers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ErpIntegration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();
        return services;
    }
}
