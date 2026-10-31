using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Domain.Entities;

/// <summary>
/// An invoice raised against a <see cref="Customer"/> and synchronized to the same ERP
/// the customer belongs to. Carries the customer's ERP id so the ERP can link the invoice.
/// </summary>
public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }

    /// <summary>The customer's ERP identifier, captured when the invoice is synced.</summary>
    public string? CustomerErpId { get; set; }

    public string Number { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public ErpSystem TargetErp { get; set; }

    /// <summary>Identifier assigned by the ERP once the invoice is accepted.</summary>
    public string? ErpInvoiceId { get; set; }

    public SyncStatus SyncStatus { get; set; } = SyncStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public void MarkSynced(string erpInvoiceId)
    {
        ErpInvoiceId = erpInvoiceId;
        SyncStatus = SyncStatus.Synced;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed()
    {
        SyncStatus = SyncStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
