using ErpIntegration.Application.Abstractions;
using ErpIntegration.Application.Common;
using ErpIntegration.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ErpIntegration.Application.Customers;

/// <summary>
/// Persists an internal customer, pushes it to the target ERP, and records an audit
/// log for the attempt. The sync state is derived from the connector outcome, so a
/// failed ERP call still persists the customer as <c>Failed</c> for later retry.
/// </summary>
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IErpConnectorFactory _connectorFactory;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork,
        IErpConnectorFactory connectorFactory,
        ILogger<CustomerService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _connectorFactory = connectorFactory;
        _logger = logger;
    }

    public async Task<CustomerDto> CreateAndSyncAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var customer = CustomerMapper.ToEntity(request);
        await _repository.AddAsync(customer, ct);

        var connector = _connectorFactory.Create(customer.TargetErp);
        var result = await connector.PushCustomerAsync(customer, ct);

        if (result.Success)
        {
            customer.MarkSynced(result.ErpReference!);
            _logger.LogInformation("Customer {CustomerId} synced to {Erp} as {Reference}",
                customer.Id, customer.TargetErp, result.ErpReference);
        }
        else
        {
            customer.MarkFailed();
            _logger.LogWarning("Customer {CustomerId} failed to sync to {Erp}: {Message}",
                customer.Id, customer.TargetErp, result.Message);
        }

        _repository.AddSyncLog(new ErpSyncLog
        {
            CustomerId = customer.Id,
            TargetErp = customer.TargetErp,
            Result = customer.SyncStatus,
            Attempts = result.Attempts,
            ErpReference = result.ErpReference,
            Message = result.Message
        });

        await _unitOfWork.SaveChangesAsync(ct);
        return CustomerMapper.ToDto(customer);
    }

    public async Task<CustomerDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Customer '{id}' was not found.");
        return CustomerMapper.ToDto(customer);
    }

    public async Task<IReadOnlyList<CustomerDto>> ListAsync(CancellationToken ct = default)
    {
        var customers = await _repository.ListAsync(ct);
        return customers.Select(CustomerMapper.ToDto).ToList();
    }
}
