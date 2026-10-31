using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Application.Customers;

/// <summary>Outbound representation of a customer and its ERP sync state.</summary>
public sealed class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public ErpSystem TargetErp { get; set; }
    public string? ErpCustomerId { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
