using ErpIntegration.Application.Models;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Application.Abstractions;

/// <summary>Pushes internal records into a specific ERP platform.</summary>
public interface IErpConnector
{
    /// <summary>The ERP this connector targets.</summary>
    ErpSystem System { get; }

    /// <summary>Maps and transmits a customer to the ERP, returning the outcome.</summary>
    Task<ErpSyncResult> PushCustomerAsync(Customer customer, CancellationToken ct = default);

    /// <summary>Maps and transmits an invoice to the ERP, returning the outcome.</summary>
    Task<ErpSyncResult> PushInvoiceAsync(Invoice invoice, CancellationToken ct = default);
}
