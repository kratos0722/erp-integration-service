using ErpIntegration.Application.Invoices;
using FluentAssertions;
using Xunit;

namespace ErpIntegration.UnitTests;

public class CreateInvoiceRequestValidatorTests
{
    private readonly CreateInvoiceRequestValidator _validator = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _validator.Validate(new CreateInvoiceRequest
        {
            CustomerId = Guid.NewGuid(), Number = "INV-1001", Amount = 250.00m, Currency = "USD"
        });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("INV-1", 0, "USD")]       // non-positive amount
    [InlineData("INV-1", 100, "US")]      // bad currency length
    [InlineData("", 100, "USD")]          // missing number
    public void Invalid_request_fails(string number, decimal amount, string currency)
    {
        var result = _validator.Validate(new CreateInvoiceRequest
        {
            CustomerId = Guid.NewGuid(), Number = number, Amount = amount, Currency = currency
        });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_customer_id_fails()
    {
        var result = _validator.Validate(new CreateInvoiceRequest
        {
            CustomerId = Guid.Empty, Number = "INV-1", Amount = 10m, Currency = "USD"
        });
        result.IsValid.Should().BeFalse();
    }
}
