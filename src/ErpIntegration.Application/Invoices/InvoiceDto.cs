using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Application.Invoices;

/// <summary>Outbound representation of an invoice and its ERP sync state.</summary>
public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Number { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public ErpSystem TargetErp { get; set; }
    public string? ErpInvoiceId { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
