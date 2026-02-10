using MediatR;

namespace Parameters.Application.Features.GetParameterByStamp;

/// <summary>
/// Query para obter Parâmetro por Stamp
/// </summary>
public record GetParameterByStampQuery(string E1Stamp) : IRequest<ParameterDto?>;
