using ErpIntegration.Application.Abstractions;
using ErpIntegration.Application.Models;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ErpIntegration.Infrastructure.Erp;

/// <summary>
/// Shared ERP push behaviour: invokes the ERP-specific <see cref="SendAsync"/>, retrying
/// transient failures with exponential backoff, and translates the outcome into an
/// <see cref="ErpSyncResult"/>. Permanent rejections are not retried.
/// </summary>
public abstract class ErpConnectorBase : IErpConnector
{
    private readonly ILogger _logger;
    private readonly ResiliencePipeline _retryPipeline;

    protected ErpConnectorBase(ILogger logger)
    {
        _logger = logger;
        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<ErpTransientException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(50),
                BackoffType = DelayBackoffType.Exponential
            })
            .Build();
    }

    public abstract ErpSystem System { get; }

    /// <summary>
    /// Maps the internal entity to the ERP payload and transmits it, returning the ERP
    /// reference. Throw <see cref="ErpTransientException"/> for retryable faults and
    /// <see cref="ErpRejectedException"/> for permanent ones.
    /// </summary>
    protected abstract Task<string> SendAsync(Customer customer, CancellationToken ct);

    public async Task<ErpSyncResult> PushCustomerAsync(Customer customer, CancellationToken ct = default)
    {
        var attempts = 0;
        try
        {
            var reference = await _retryPipeline.ExecuteAsync(async token =>
            {
                attempts++;
                return await SendAsync(customer, token);
            }, ct);

            return ErpSyncResult.Ok(reference, attempts);
        }
        catch (ErpTransientException ex)
        {
            _logger.LogError(ex, "ERP {Erp} push exhausted retries for customer {CustomerId}", System, customer.Id);
            return ErpSyncResult.Fail($"Transient failure after {attempts} attempts: {ex.Message}", attempts);
        }
        catch (ErpRejectedException ex)
        {
            _logger.LogWarning(ex, "ERP {Erp} rejected customer {CustomerId}", System, customer.Id);
            return ErpSyncResult.Fail($"Rejected by {System}: {ex.Message}", attempts);
        }
    }
}
