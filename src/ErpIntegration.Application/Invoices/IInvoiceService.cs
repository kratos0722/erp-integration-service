namespace ErpIntegration.Application.Invoices;

public interface IInvoiceService
{
    Task<InvoiceDto> CreateAndSyncAsync(CreateInvoiceRequest request, CancellationToken ct = default);
    Task<InvoiceDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<InvoiceDto>> ListAsync(CancellationToken ct = default);
}
