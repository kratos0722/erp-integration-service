using ErpIntegration.Application.Invoices;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ErpIntegration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly IValidator<CreateInvoiceRequest> _validator;

    public InvoicesController(IInvoiceService service, IValidator<CreateInvoiceRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>Creates an invoice for an existing (already-synced) customer and pushes it to the ERP.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);
        var dto = await _service.CreateAndSyncAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Returns a single invoice by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _service.GetAsync(id, ct));

    /// <summary>Lists all invoices, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> List(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));
}
