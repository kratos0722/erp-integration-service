using ErpIntegration.Domain.Entities;

namespace ErpIntegration.Application.Abstractions;

/// <summary>Persistence boundary for customers and their sync audit log.</summary>
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default);
    void AddSyncLog(ErpSyncLog log);
}
