using MediatR;

namespace Parameters.Application.Features.CreateParameter;

/// <summary>
/// Command para criar um novo Parâmetro
/// </summary>
public record CreateParameterCommand(
    string Code,
    string Description,
    CreateE4Dto? E4Details,
    string? CreatedBy
) : IRequest<ParameterDto>;
