using FluentValidation;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateProduct;

namespace FSH.Modules.Products.Application.Features.v1.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("Product Id must be greater than 0.");

        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256).WithMessage("Title must not exceed 256 characters.");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(p => !string.IsNullOrEmpty(p.Description));

        RuleFor(p => p.Revision)
            .GreaterThanOrEqualTo(0).WithMessage("Revision must be greater than or equal to 0.");

        RuleFor(p => p.Variant)
            .MaximumLength(100).WithMessage("Variant must not exceed 100 characters.");

        RuleFor(p => p.Version)
            .GreaterThanOrEqualTo(0).WithMessage("Version must be greater than or equal to 0.");

        RuleFor(p => p.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than 0.")
            .When(p => p.CategoryId.HasValue);

        RuleFor(p => p.Keywords)
            .MaximumLength(500).WithMessage("Keywords must not exceed 500 characters.")
            .When(p => !string.IsNullOrEmpty(p.Keywords));

        RuleFor(p => p.Language)
            .MaximumLength(50).WithMessage("Language must not exceed 50 characters.")
            .When(p => !string.IsNullOrEmpty(p.Language));

        RuleFor(p => p.Subject)
            .MaximumLength(200).WithMessage("Subject must not exceed 200 characters.")
            .When(p => !string.IsNullOrEmpty(p.Subject));

        RuleFor(p => p.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
            .When(p => !string.IsNullOrEmpty(p.Notes));

        RuleFor(p => p.HashSha256)
            .Matches(@"^[a-fA-F0-9]{64}$").WithMessage("HashSha256 must be a valid SHA-256 hash (64 hexadecimal characters).")
            .When(p => !string.IsNullOrEmpty(p.HashSha256));

        RuleFor(p => p.CustomerId)
            .NotEmpty().WithMessage("CustomerId cannot be empty.")
            .When(p => !string.IsNullOrEmpty(p.CustomerId));

        // Mirroring validation
        RuleFor(p => p.Mirroring)
            .Must(m => m == null || !m.IsMirrored || !string.IsNullOrWhiteSpace(m.SourceProductTitle))
            .WithMessage("SourceProductTitle is required when IsMirrored is true.")
            .When(p => p.Mirroring is not null);
    }
}
