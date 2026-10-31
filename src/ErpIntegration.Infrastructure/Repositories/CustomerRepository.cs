using ErpIntegration.Application.Abstractions;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpIntegration.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ErpDbContext _db;
    public CustomerRepository(ErpDbContext db) => _db = db;

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
        => await _db.Customers.AddAsync(customer, ct);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
        => await _db.Customers.AsNoTracking().OrderByDescending(c => c.CreatedAt).ToListAsync(ct);

    public void AddSyncLog(ErpSyncLog log) => _db.SyncLogs.Add(log);
}
