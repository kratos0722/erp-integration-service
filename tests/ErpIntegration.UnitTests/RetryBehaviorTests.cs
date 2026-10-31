using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using ErpIntegration.Infrastructure.Erp;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ErpIntegration.UnitTests;

public class RetryBehaviorTests
{
    /// <summary>Test connector that fails transiently a set number of times before succeeding.</summary>
    private sealed class FlakyConnector(int failBefore) : ErpConnectorBase(NullLogger.Instance)
    {
        private int _calls;
        public override ErpSystem System => ErpSystem.Sage300;

        protected override Task<string> SendCustomerAsync(Customer customer, CancellationToken ct)
        {
            if (++_calls <= failBefore) throw new ErpTransientException($"attempt {_calls}");
            return Task.FromResult("AR-999999");
        }

        protected override Task<string> SendInvoiceAsync(Invoice invoice, CancellationToken ct)
            => Task.FromResult("ARINV-999999");
    }

    [Fact]
    public async Task Transient_failures_are_retried_then_succeed()
    {
        var connector = new FlakyConnector(failBefore: 2);

        var result = await connector.PushCustomerAsync(new Customer { Name = "X", Email = "x@x.com" });

        result.Success.Should().BeTrue();
        result.Attempts.Should().Be(3); // 2 transient failures + 1 success
    }

    [Fact]
    public async Task Exhausted_retries_return_failure()
    {
        var connector = new FlakyConnector(failBefore: 99);

        var result = await connector.PushCustomerAsync(new Customer { Name = "X", Email = "x@x.com" });

        result.Success.Should().BeFalse();
        result.Attempts.Should().Be(4); // initial attempt + 3 retries
    }
}
