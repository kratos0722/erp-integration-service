namespace ErpIntegration.Application.Customers;

public interface ICustomerService
{
    Task<CustomerDto> CreateAndSyncAsync(CreateCustomerRequest request, CancellationToken ct = default);
    Task<CustomerDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CustomerDto>> ListAsync(CancellationToken ct = default);
}
