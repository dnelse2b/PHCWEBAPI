using MediatR;
using Parameters.Domain.Repositories;
using Parameters.Application.Mappings;

namespace Parameters.Application.Features.GetAllParameters;

public class GetAllParametersQueryHandler : IRequestHandler<GetAllParametersQuery, IEnumerable<ParameterDto>>
{
    private readonly IPara1Repository _para1Repository;

    public GetAllParametersQueryHandler(IPara1Repository para1Repository)
    {
        _para1Repository = para1Repository;
    }

    public async Task<IEnumerable<ParameterDto>> Handle(GetAllParametersQuery request, CancellationToken cancellationToken)
    {
        var para1List = await _para1Repository.GetAllAsync(request.IncludeInactive, cancellationToken);

        return para1List.ToDtos<ParameterDto>();
    }
}

