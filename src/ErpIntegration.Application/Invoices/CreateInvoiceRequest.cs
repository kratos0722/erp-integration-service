namespace ErpIntegration.Application.Invoices;

/// <summary>Inbound payload for creating and syncing an invoice for an existing customer.</summary>
public sealed class CreateInvoiceRequest
{
    public Guid CustomerId { get; set; }
    public string Number { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}
