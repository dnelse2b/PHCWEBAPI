using MediatR;

namespace Parameters.Application.Features.GetAllParameters;

/// <summary>
/// Query para obter todos os Parâmetros
/// </summary>
public record GetAllParametersQuery(bool IncludeInactive = false) : IRequest<IEnumerable<ParameterDto>>;
