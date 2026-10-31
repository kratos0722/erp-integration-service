using ErpIntegration.Domain.Entities;

namespace ErpIntegration.Application.Invoices;

/// <summary>Maps between the internal invoice schema and the API representation.</summary>
public static class InvoiceMapper
{
    public static Invoice ToEntity(CreateInvoiceRequest r) => new()
    {
        CustomerId = r.CustomerId,
        Number = r.Number.Trim(),
        Amount = r.Amount,
        Currency = r.Currency.Trim().ToUpperInvariant()
    };

    public static InvoiceDto ToDto(Invoice i) => new()
    {
        Id = i.Id,
        CustomerId = i.CustomerId,
        Number = i.Number,
        Amount = i.Amount,
        Currency = i.Currency,
        TargetErp = i.TargetErp,
        ErpInvoiceId = i.ErpInvoiceId,
        SyncStatus = i.SyncStatus.ToString(),
        CreatedAt = i.CreatedAt
    };
}
