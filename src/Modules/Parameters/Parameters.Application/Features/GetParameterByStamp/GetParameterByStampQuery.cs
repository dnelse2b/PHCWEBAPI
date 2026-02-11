using MediatR;

namespace Parameters.Application.Features.GetParameterByStamp;


public record GetParameterByStampQuery(string ParaStamp) : IRequest<ParameterDto?>;
