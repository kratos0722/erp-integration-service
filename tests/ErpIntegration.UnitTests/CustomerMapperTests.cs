using ErpIntegration.Application.Customers;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ErpIntegration.UnitTests;

public class CustomerMapperTests
{
    [Fact]
    public void ToEntity_trims_and_normalizes_email()
    {
        var entity = CustomerMapper.ToEntity(new CreateCustomerRequest
        {
            Name = "  Acme  ", Email = "  Billing@ACME.com ", TargetErp = ErpSystem.Acumatica
        });

        entity.Name.Should().Be("Acme");
        entity.Email.Should().Be("billing@acme.com");
        entity.TargetErp.Should().Be(ErpSystem.Acumatica);
    }
}
