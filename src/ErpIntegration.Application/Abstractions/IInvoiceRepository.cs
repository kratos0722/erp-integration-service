using ErpIntegration.Domain.Entities;

namespace ErpIntegration.Application.Abstractions;

/// <summary>Persistence boundary for invoices.</summary>
public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> ListAsync(CancellationToken ct = default);
}
