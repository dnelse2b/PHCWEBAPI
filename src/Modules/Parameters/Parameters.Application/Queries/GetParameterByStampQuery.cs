using MediatR;
using Parameters.Application.DTOs;

namespace Parameters.Application.Queries;

/// <summary>
/// Query para obter Parâmetro por Stamp
/// </summary>
public record GetParameterByStampQuery(string E1Stamp) : IRequest<ParameterDto?>;
