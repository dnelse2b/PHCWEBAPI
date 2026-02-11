using MediatR;

namespace Parameters.Application.Features.CreateParameter;

public record CreateParameterCommand(
    CreateParameterDto Dto,
    string? CriadoPor
) : IRequest<ParameterDto>;
