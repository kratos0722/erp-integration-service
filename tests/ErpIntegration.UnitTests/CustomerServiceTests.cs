using ErpIntegration.Application.Abstractions;
using ErpIntegration.Application.Customers;
using ErpIntegration.Application.Models;
using ErpIntegration.Domain.Entities;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ErpIntegration.UnitTests;

public class CustomerServiceTests
{
    private static CustomerService Build(Mock<IErpConnector> connector,
        Mock<ICustomerRepository>? repo = null, Mock<IUnitOfWork>? uow = null)
    {
        repo ??= new Mock<ICustomerRepository>();
        uow ??= new Mock<IUnitOfWork>();
        var factory = new Mock<IErpConnectorFactory>();
        factory.Setup(f => f.Create(It.IsAny<ErpSystem>())).Returns(connector.Object);
        return new CustomerService(repo.Object, uow.Object, factory.Object, NullLogger<CustomerService>.Instance);
    }

    [Fact]
    public async Task CreateAndSync_marks_synced_and_persists_on_success()
    {
        var connector = new Mock<IErpConnector>();
        connector.Setup(c => c.PushCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ErpSyncResult.Ok("AR-000123", 1));
        var repo = new Mock<ICustomerRepository>();
        var uow = new Mock<IUnitOfWork>();

        var service = Build(connector, repo, uow);

        var dto = await service.CreateAndSyncAsync(new CreateCustomerRequest
        {
            Name = "Acme", Email = "a@acme.com", TargetErp = ErpSystem.Sage300
        });

        dto.SyncStatus.Should().Be("Synced");
        dto.ErpCustomerId.Should().Be("AR-000123");
        repo.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddSyncLog(It.IsAny<ErpSyncLog>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAndSync_marks_failed_when_erp_push_fails()
    {
        var connector = new Mock<IErpConnector>();
        connector.Setup(c => c.PushCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(ErpSyncResult.Fail("ERP unavailable", 4));

        var service = Build(connector);

        var dto = await service.CreateAndSyncAsync(new CreateCustomerRequest
        {
            Name = "Acme", Email = "a@acme.com", TargetErp = ErpSystem.Sage300
        });

        dto.SyncStatus.Should().Be("Failed");
        dto.ErpCustomerId.Should().BeNull();
    }
}
