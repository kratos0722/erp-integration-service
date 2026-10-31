using ErpIntegration.Domain.Enums;

namespace ErpIntegration.Application.Abstractions;

/// <summary>Resolves the <see cref="IErpConnector"/> registered for a given ERP.</summary>
public interface IErpConnectorFactory
{
    IErpConnector Create(ErpSystem system);
}
