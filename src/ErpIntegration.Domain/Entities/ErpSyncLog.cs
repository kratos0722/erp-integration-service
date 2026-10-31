using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Domain.Entities;

/// <summary>Audit-trail entry recording every attempt to push a record to an ERP.</summary>
public class ErpSyncLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }

    /// <summary>Set when the logged attempt was an invoice push rather than a customer push.</summary>
    public Guid? InvoiceId { get; set; }

    public ErpSystem TargetErp { get; set; }
    public SyncStatus Result { get; set; }
    public int Attempts { get; set; }
    public string? ErpReference { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
