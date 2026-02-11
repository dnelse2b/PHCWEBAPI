using MediatR;
using Parameters.Domain.Repositories;
using Parameters.Application.Mappings;

namespace Parameters.Application.Features.GetParameterByStamp;

public class GetParameterByStampQueryHandler : IRequestHandler<GetParameterByStampQuery, ParameterDto?>
{
    private readonly IPara1Repository _para1Repository;

    public GetParameterByStampQueryHandler(IPara1Repository para1Repository)
    {
        _para1Repository = para1Repository;
    }

    public async Task<ParameterDto?> Handle(GetParameterByStampQuery request, CancellationToken cancellationToken)
    {
        var para1 = await _para1Repository.GetByStampAsync(request.ParaStamp, cancellationToken);

        return para1?.ToDto<ParameterDto>();
    }
}

