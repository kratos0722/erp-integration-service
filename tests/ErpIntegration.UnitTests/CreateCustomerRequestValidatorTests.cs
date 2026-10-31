using ErpIntegration.Application.Customers;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ErpIntegration.UnitTests;

public class CreateCustomerRequestValidatorTests
{
    private readonly CreateCustomerRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.Validate(new CreateCustomerRequest
        {
            Name = "Acme", Email = "billing@acme.com", TargetErp = ErpSystem.Sage300
        });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "a@b.com")]
    [InlineData("Acme", "")]
    [InlineData("Acme", "not-an-email")]
    public void Invalid_request_fails(string name, string email)
    {
        var result = _validator.Validate(new CreateCustomerRequest
        {
            Name = name, Email = email, TargetErp = ErpSystem.Sage300
        });
        result.IsValid.Should().BeFalse();
    }
}
