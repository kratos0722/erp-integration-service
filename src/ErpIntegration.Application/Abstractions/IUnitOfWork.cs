namespace ErpIntegration.Application.Abstractions;

/// <summary>Commits the current set of tracked changes as a single transaction.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
