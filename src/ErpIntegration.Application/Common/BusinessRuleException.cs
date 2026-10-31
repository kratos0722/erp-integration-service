namespace ErpIntegration.Application.Common;

/// <summary>Thrown when a request is well-formed but violates a business rule. Mapped to HTTP 409.</summary>
public sealed class BusinessRuleException(string message) : Exception(message);
