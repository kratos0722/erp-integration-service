using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ErpIntegration.Infrastructure.Erp;

/// <summary>
/// Simulated Sage 300 connector. A production implementation would call the Sage 300
/// Web API (AR Customers endpoint); here we map the internal schema to a Sage 300 AR
/// customer number and return it, so the sample runs without external credentials.
/// </summary>
public sealed class Sage300Connector : ErpConnectorBase
{
    public Sage300Connector(ILogger<Sage300Connector> logger) : base(logger) { }

    public override ErpSystem System => ErpSystem.Sage300;

    protected override Task<string> SendCustomerAsync(Customer customer, CancellationToken ct)
    {
        // Sage 300 rejects records without a customer name (AR business rule).
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ErpRejectedException("Sage 300 requires a customer name.");

        // Map internal id -> Sage 300 AR customer number (AR-######).
        var reference = $"AR-{(uint)customer.Id.GetHashCode() % 1_000_000:D6}";
        return Task.FromResult(reference);
    }

    protected override Task<string> SendInvoiceAsync(Invoice invoice, CancellationToken ct)
    {
        // Sage 300 A/R invoices must reference an existing customer and carry a positive amount.
        if (string.IsNullOrWhiteSpace(invoice.CustomerErpId))
            throw new ErpRejectedException("Sage 300 requires the customer's AR number before invoicing.");
        if (invoice.Amount <= 0)
            throw new ErpRejectedException("Sage 300 rejects non-positive invoice amounts.");

        // Map internal id -> Sage 300 A/R invoice number (ARINV-######).
        var reference = $"ARINV-{(uint)invoice.Id.GetHashCode() % 1_000_000:D6}";
        return Task.FromResult(reference);
    }
}
