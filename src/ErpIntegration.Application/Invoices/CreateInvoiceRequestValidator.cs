using FluentValidation;

namespace ErpIntegration.Application.Invoices;

public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter ISO code.");
    }
}
