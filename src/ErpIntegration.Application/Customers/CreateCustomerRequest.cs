using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Application.Customers;

/// <summary>Inbound payload for creating and syncing a customer.</summary>
public sealed class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public ErpSystem TargetErp { get; set; }
}
