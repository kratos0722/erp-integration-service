using ErpIntegration.Application.Abstractions;
using ErpIntegration.Application.Common;
using ErpIntegration.Application.Invoices;
using ErpIntegration.Application.Models;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ErpIntegration.UnitTests;

public class InvoiceServiceTests
{
    private static Customer SyncedCustomer() => new()
    {
        Name = "Acme", Email = "a@acme.com", TargetErp = ErpSystem.Sage300,
        ErpCustomerId = "AR-000123", SyncStatus = SyncStatus.Synced
    };

    private static InvoiceService Build(
        Mock<ICustomerRepository> customers, Mock<IErpConnector>? connector = null,
        Mock<IInvoiceRepository>? invoices = null, Mock<IUnitOfWork>? uow = null)
    {
        connector ??= new Mock<IErpConnector>();
        connector.Setup(c => c.PushInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ErpSyncResult.Ok("ARINV-000999", 1));
        var factory = new Mock<IErpConnectorFactory>();
        factory.Setup(f => f.Create(It.IsAny<ErpSystem>())).Returns(connector.Object);
        return new InvoiceService(
            (invoices ?? new Mock<IInvoiceRepository>()).Object,
            customers.Object,
            (uow ?? new Mock<IUnitOfWork>()).Object,
            factory.Object,
            NullLogger<InvoiceService>.Instance);
    }

    [Fact]
    public async Task CreateAndSync_syncs_invoice_for_synced_customer()
    {
        var customer = SyncedCustomer();
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        var uow = new Mock<IUnitOfWork>();

        var service = Build(customers, uow: uow);

        var dto = await service.CreateAndSyncAsync(new CreateInvoiceRequest
        {
            CustomerId = customer.Id, Number = "INV-1", Amount = 100m, Currency = "USD"
        });

        dto.SyncStatus.Should().Be("Synced");
        dto.ErpInvoiceId.Should().Be("ARINV-000999");
        dto.TargetErp.Should().Be(ErpSystem.Sage300);
        customers.Verify(r => r.AddSyncLog(It.Is<ErpSyncLog>(l => l.InvoiceId != null)), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAndSync_throws_when_customer_missing()
    {
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Customer?)null);

        var service = Build(customers);

        var act = () => service.CreateAndSyncAsync(new CreateInvoiceRequest
        {
            CustomerId = Guid.NewGuid(), Number = "INV-1", Amount = 100m, Currency = "USD"
        });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAndSync_throws_when_customer_not_synced()
    {
        var customer = SyncedCustomer();
        customer.SyncStatus = SyncStatus.Failed;
        customer.ErpCustomerId = null;
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        var service = Build(customers);

        var act = () => service.CreateAndSyncAsync(new CreateInvoiceRequest
        {
            CustomerId = customer.Id, Number = "INV-1", Amount = 100m, Currency = "USD"
        });

        await act.Should().ThrowAsync<BusinessRuleException>();
    }
}
