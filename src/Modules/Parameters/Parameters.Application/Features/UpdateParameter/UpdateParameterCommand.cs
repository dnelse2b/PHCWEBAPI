using MediatR;

namespace Parameters.Application.Features.UpdateParameter;

public record UpdateParameterCommand(
    string ParaStamp,
    UpdateParameterDto Dto,
    string? AtualizadoPor
) : IRequest<ParameterDto>;
