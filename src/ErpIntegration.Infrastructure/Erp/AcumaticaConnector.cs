using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ErpIntegration.Infrastructure.Erp;

/// <summary>
/// Simulated Acumatica connector. A production implementation would authenticate against
/// the Acumatica REST contract-based API and PUT a Customer entity; here we map the
/// internal schema to an Acumatica customer id (CUST######) and return it.
/// </summary>
public sealed class AcumaticaConnector : ErpConnectorBase
{
    public AcumaticaConnector(ILogger<AcumaticaConnector> logger) : base(logger) { }

    public override ErpSystem System => ErpSystem.Acumatica;

    protected override Task<string> SendCustomerAsync(Customer customer, CancellationToken ct)
    {
        // Acumatica requires an email for customer contact creation.
        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ErpRejectedException("Acumatica requires a contact email.");

        var reference = $"CUST{(uint)customer.Id.GetHashCode() % 1_000_000:D6}";
        return Task.FromResult(reference);
    }

    protected override Task<string> SendInvoiceAsync(Invoice invoice, CancellationToken ct)
    {
        // Acumatica links an AR invoice to an existing customer id.
        if (string.IsNullOrWhiteSpace(invoice.CustomerErpId))
            throw new ErpRejectedException("Acumatica requires the customer id before invoicing.");
        if (invoice.Amount <= 0)
            throw new ErpRejectedException("Acumatica rejects non-positive invoice amounts.");

        var reference = $"INV{(uint)invoice.Id.GetHashCode() % 1_000_000:D6}";
        return Task.FromResult(reference);
    }
}
