using ErpIntegration.Application.Customers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ErpIntegration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly IValidator<CreateCustomerRequest> _validator;

    public CustomersController(ICustomerService service, IValidator<CreateCustomerRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>Creates a customer and synchronizes it to the configured ERP.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);
        var dto = await _service.CreateAndSyncAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Returns a single customer by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _service.GetAsync(id, ct));

    /// <summary>Lists all customers, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> List(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));
}
