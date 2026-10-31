using System.Net;
using System.Net.Http.Json;
using ErpIntegration.Application.Customers;
using ErpIntegration.Application.Invoices;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ErpIntegration.IntegrationTests;

public sealed class InvoicesApiTests : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client;
    public InvoicesApiTests(CustomApiFactory factory) => _client = factory.CreateClient();

    private async Task<CustomerDto> CreateSyncedCustomerAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest
        {
            Name = "Acme", Email = "ap@acme.com", TargetErp = ErpSystem.Sage300
        });
        return (await response.Content.ReadFromJsonAsync<CustomerDto>())!;
    }

    [Fact]
    public async Task Create_invoice_for_synced_customer_returns_201_and_syncs()
    {
        var customer = await CreateSyncedCustomerAsync();

        var response = await _client.PostAsJsonAsync("/api/invoices", new CreateInvoiceRequest
        {
            CustomerId = customer.Id, Number = "INV-1001", Amount = 500m, Currency = "USD"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        dto!.SyncStatus.Should().Be("Synced");
        dto.ErpInvoiceId.Should().StartWith("ARINV-");
        dto.TargetErp.Should().Be(ErpSystem.Sage300);
    }

    [Fact]
    public async Task Create_invoice_for_unknown_customer_returns_404()
    {
        var response = await _client.PostAsJsonAsync("/api/invoices", new CreateInvoiceRequest
        {
            CustomerId = Guid.NewGuid(), Number = "INV-1", Amount = 100m, Currency = "USD"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_invoice_with_invalid_amount_returns_400()
    {
        var customer = await CreateSyncedCustomerAsync();

        var response = await _client.PostAsJsonAsync("/api/invoices", new CreateInvoiceRequest
        {
            CustomerId = customer.Id, Number = "INV-1", Amount = 0m, Currency = "USD"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_then_get_round_trips_the_invoice()
    {
        var customer = await CreateSyncedCustomerAsync();
        var create = await _client.PostAsJsonAsync("/api/invoices", new CreateInvoiceRequest
        {
            CustomerId = customer.Id, Number = "INV-2002", Amount = 75.50m, Currency = "EUR"
        });
        var created = await create.Content.ReadFromJsonAsync<InvoiceDto>();

        var get = await _client.GetAsync($"/api/invoices/{created!.Id}");
        var fetched = await get.Content.ReadFromJsonAsync<InvoiceDto>();

        get.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched!.Number.Should().Be("INV-2002");
        fetched.Currency.Should().Be("EUR");
    }
}
