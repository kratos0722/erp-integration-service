using ErpIntegration.Application.Abstractions;
using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Infrastructure.Erp;

/// <summary>
/// Resolves a connector by ERP from all registered <see cref="IErpConnector"/> instances.
/// Adding a new ERP is a matter of registering another connector — no change here.
/// </summary>
public sealed class ErpConnectorFactory : IErpConnectorFactory
{
    private readonly IReadOnlyDictionary<ErpSystem, IErpConnector> _connectors;

    public ErpConnectorFactory(IEnumerable<IErpConnector> connectors)
        => _connectors = connectors.ToDictionary(c => c.System);

    public IErpConnector Create(ErpSystem system)
        => _connectors.TryGetValue(system, out var connector)
            ? connector
            : throw new NotSupportedException($"No connector registered for ERP '{system}'.");
}
