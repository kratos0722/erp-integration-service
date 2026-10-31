using ErpIntegration.Application.Abstractions;
using ErpIntegration.Application.Common;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ErpIntegration.Application.Invoices;

/// <summary>
/// Creates an invoice for an existing customer and syncs it to that customer's ERP.
/// Enforces the rule that a customer must already be synced to the ERP before any of
/// their invoices can be pushed, and records an audit log for the attempt.
/// </summary>
public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoices;
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IErpConnectorFactory _connectorFactory;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository invoices,
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IErpConnectorFactory connectorFactory,
        ILogger<InvoiceService> logger)
    {
        _invoices = invoices;
        _customers = customers;
        _unitOfWork = unitOfWork;
        _connectorFactory = connectorFactory;
        _logger = logger;
    }

    public async Task<InvoiceDto> CreateAndSyncAsync(CreateInvoiceRequest request, CancellationToken ct = default)
    {
        var customer = await _customers.GetByIdAsync(request.CustomerId, ct)
            ?? throw new NotFoundException($"Customer '{request.CustomerId}' was not found.");

        if (customer.SyncStatus != SyncStatus.Synced || string.IsNullOrEmpty(customer.ErpCustomerId))
            throw new BusinessRuleException(
                $"Customer '{customer.Id}' must be synced to {customer.TargetErp} before invoices can be pushed.");

        var invoice = InvoiceMapper.ToEntity(request);
        invoice.TargetErp = customer.TargetErp;
        invoice.CustomerErpId = customer.ErpCustomerId;
        await _invoices.AddAsync(invoice, ct);

        var connector = _connectorFactory.Create(invoice.TargetErp);
        var result = await connector.PushInvoiceAsync(invoice, ct);

        if (result.Success)
        {
            invoice.MarkSynced(result.ErpReference!);
            _logger.LogInformation("Invoice {InvoiceId} synced to {Erp} as {Reference}",
                invoice.Id, invoice.TargetErp, result.ErpReference);
        }
        else
        {
            invoice.MarkFailed();
            _logger.LogWarning("Invoice {InvoiceId} failed to sync to {Erp}: {Message}",
                invoice.Id, invoice.TargetErp, result.Message);
        }

        _customers.AddSyncLog(new ErpSyncLog
        {
            CustomerId = invoice.CustomerId,
            InvoiceId = invoice.Id,
            TargetErp = invoice.TargetErp,
            Result = invoice.SyncStatus,
            Attempts = result.Attempts,
            ErpReference = result.ErpReference,
            Message = result.Message
        });

        await _unitOfWork.SaveChangesAsync(ct);
        return InvoiceMapper.ToDto(invoice);
    }

    public async Task<InvoiceDto> GetAsync(Guid id, CancellationToken ct = default)
    {
        var invoice = await _invoices.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Invoice '{id}' was not found.");
        return InvoiceMapper.ToDto(invoice);
    }

    public async Task<IReadOnlyList<InvoiceDto>> ListAsync(CancellationToken ct = default)
    {
        var invoices = await _invoices.ListAsync(ct);
        return invoices.Select(InvoiceMapper.ToDto).ToList();
    }
}
