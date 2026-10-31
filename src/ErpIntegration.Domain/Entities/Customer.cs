using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Domain.Entities;

/// <summary>
/// Internal representation of a customer that is kept in sync with an external ERP
/// (Sage 300 / Acumatica). The entity owns its own sync-state transitions.
/// </summary>
public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }

    public ErpSystem TargetErp { get; set; }

    /// <summary>Identifier assigned by the ERP once the record is accepted.</summary>
    public string? ErpCustomerId { get; set; }

    public SyncStatus SyncStatus { get; set; } = SyncStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public void MarkSynced(string erpCustomerId)
    {
        ErpCustomerId = erpCustomerId;
        SyncStatus = SyncStatus.Synced;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed()
    {
        SyncStatus = SyncStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
