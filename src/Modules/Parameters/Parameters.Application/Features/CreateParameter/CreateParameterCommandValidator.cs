using FluentValidation;

namespace Parameters.Application.Features.CreateParameter;

public class CreateParameterCommandValidator : AbstractValidator<CreateParameterCommand>
{
    public CreateParameterCommandValidator()
    {
        RuleFor(x => x.Dto.Descricao)
            .NotEmpty().WithMessage("Descricao is required")
            .MaximumLength(200).WithMessage("Descricao must not exceed 200 characters");

        RuleFor(x => x.Dto.Valor)
            .NotEmpty().WithMessage("Valor is required")
            .MaximumLength(500).WithMessage("Valor must not exceed 500 characters");

        RuleFor(x => x.Dto.Tipo)
            .NotEmpty().WithMessage("Tipo is required")
            .MaximumLength(50).WithMessage("Tipo must not exceed 50 characters");

        RuleFor(x => x.Dto.Dec)
            .GreaterThanOrEqualTo(0).WithMessage("Dec must be greater than or equal to 0")
            .When(x => x.Dto.Dec.HasValue);

        RuleFor(x => x.Dto.Tam)
            .GreaterThanOrEqualTo(0).WithMessage("Tam must be greater than 0")
            .When(x => x.Dto.Tam.HasValue);
    }
}
