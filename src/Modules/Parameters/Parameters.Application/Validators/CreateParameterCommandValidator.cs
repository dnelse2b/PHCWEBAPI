using FluentValidation;
using Parameters.Application.Commands;

namespace Parameters.Application.Validators;

/// <summary>
/// Validator para CreateParameterCommand
/// </summary>
public class CreateParameterCommandValidator : AbstractValidator<CreateParameterCommand>
{
    public CreateParameterCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.E4Details)
            .SetValidator(new CreateE4DtoValidator()!)
            .When(x => x.E4Details != null);
    }
}

/// <summary>
/// Validator para CreateE4Dto
/// </summary>
public class CreateE4DtoValidator : AbstractValidator<DTOs.CreateE4Dto>
{
    public CreateE4DtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required")
            .MaximumLength(200).WithMessage("Value must not exceed 200 characters");

        RuleFor(x => x.Sequence)
            .GreaterThanOrEqualTo(0).WithMessage("Sequence must be greater than or equal to 0");

        RuleFor(x => x.AdditionalInfo)
            .MaximumLength(1000).WithMessage("AdditionalInfo must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdditionalInfo));
    }
}
