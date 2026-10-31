using System.Net;
using System.Net.Http.Json;
using ErpIntegration.Application.Customers;
using ErpIntegration.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ErpIntegration.IntegrationTests;

public sealed class CustomersApiTests : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client;
    public CustomersApiTests(CustomApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Create_valid_customer_returns_201_and_syncs_to_sage()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Contoso Ltd", Email = "ap@contoso.com", TargetErp = ErpSystem.Sage300
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<CustomerDto>();
        dto.Should().NotBeNull();
        dto!.SyncStatus.Should().Be("Synced");
        dto.ErpCustomerId.Should().StartWith("AR-");
    }

    [Fact]
    public async Task Create_valid_customer_syncs_to_acumatica()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Northwind", Email = "ap@northwind.com", TargetErp = ErpSystem.Acumatica
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var dto = await response.Content.ReadFromJsonAsync<CustomerDto>();

        dto!.ErpCustomerId.Should().StartWith("CUST");
    }

    [Fact]
    public async Task Create_invalid_email_returns_400()
    {
        var request = new CreateCustomerRequest
        {
            Name = "Bad", Email = "not-an-email", TargetErp = ErpSystem.Acumatica
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_unknown_id_returns_404()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_then_get_round_trips_the_customer()
    {
        var create = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest
        {
            Name = "Globex", Email = "ap@globex.com", TargetErp = ErpSystem.Sage300
        });
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();

        var get = await _client.GetAsync($"/api/customers/{created!.Id}");
        var fetched = await get.Content.ReadFromJsonAsync<CustomerDto>();

        get.StatusCode.Should().Be(HttpStatusCode.OK);
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be("Globex");
    }
}
