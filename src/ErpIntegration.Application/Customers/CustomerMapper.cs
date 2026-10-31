using ErpIntegration.Domain.Entities;

namespace ErpIntegration.Application.Customers;

/// <summary>
/// Maps between the internal customer schema and the API representation.
/// Normalizes inbound data (trimming, lower-casing email) so downstream ERP
/// mapping receives clean values — a common source of ERP rejections.
/// </summary>
public static class CustomerMapper
{
    public static Customer ToEntity(CreateCustomerRequest r) => new()
    {
        Name = r.Name.Trim(),
        Email = r.Email.Trim().ToLowerInvariant(),
        Phone = r.Phone?.Trim(),
        AddressLine1 = r.AddressLine1?.Trim(),
        City = r.City?.Trim(),
        Country = r.Country?.Trim(),
        PostalCode = r.PostalCode?.Trim(),
        TargetErp = r.TargetErp
    };

    public static CustomerDto ToDto(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        TargetErp = c.TargetErp,
        ErpCustomerId = c.ErpCustomerId,
        SyncStatus = c.SyncStatus.ToString(),
        CreatedAt = c.CreatedAt
    };
}
