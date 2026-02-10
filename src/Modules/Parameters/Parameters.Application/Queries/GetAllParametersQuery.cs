using MediatR;
using Parameters.Application.DTOs;

namespace Parameters.Application.Queries;

/// <summary>
/// Query para obter todos os Parâmetros
/// </summary>
public record GetAllParametersQuery(bool IncludeInactive = false) : IRequest<IEnumerable<ParameterDto>>;
