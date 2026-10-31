using ErpIntegration.Application.Abstractions;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpIntegration.Infrastructure.Repositories;

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly ErpDbContext _db;
    public InvoiceRepository(ErpDbContext db) => _db = db;

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
        => await _db.Invoices.AddAsync(invoice, ct);

    public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Invoices.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<Invoice>> ListAsync(CancellationToken ct = default)
        => await _db.Invoices.AsNoTracking().OrderByDescending(i => i.CreatedAt).ToListAsync(ct);
}
