namespace ErpIntegration.Infrastructure.Erp;

/// <summary>A retryable ERP fault (timeout, 5xx, throttling). Retried with backoff.</summary>
public sealed class ErpTransientException(string message) : Exception(message);

/// <summary>A permanent ERP rejection (validation / business rule). Not retried.</summary>
public sealed class ErpRejectedException(string message) : Exception(message);
