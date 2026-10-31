namespace ErpIntegration.Application.Models;

/// <summary>Outcome of a single ERP push, returned by an <see cref="Abstractions.IErpConnector"/>.</summary>
public sealed record ErpSyncResult(bool Success, string? ErpReference, int Attempts, string? Message)
{
    public static ErpSyncResult Ok(string erpReference, int attempts) =>
        new(true, erpReference, attempts, null);

    public static ErpSyncResult Fail(string message, int attempts) =>
        new(false, null, attempts, message);
}
