using MediatR;

namespace Parameters.Application.Features.UpdateParameter;

/// <summary>
/// Command para atualizar Parâmetro
/// </summary>
public record UpdateParameterCommand(
    string E1Stamp,
    string Code,
    string Description,
    bool Active,
    UpdateE4Dto? E4Details,
    string? UpdatedBy = null
) : IRequest<ParameterDto>;
