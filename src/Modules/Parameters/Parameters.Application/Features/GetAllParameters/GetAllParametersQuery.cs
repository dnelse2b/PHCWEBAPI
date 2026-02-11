using MediatR;

namespace Parameters.Application.Features.GetAllParameters;


public record GetAllParametersQuery(bool IncludeInactive = false) : IRequest<IEnumerable<ParameterDto>>;
