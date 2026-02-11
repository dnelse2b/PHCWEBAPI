using MediatR;
using Parameters.Domain.Repositories;
using Parameters.Application.Mappings;

namespace Parameters.Application.Features.UpdateParameter;

public class UpdateParameterCommandHandler : IRequestHandler<UpdateParameterCommand, ParameterDto>
{
    private readonly IPara1Repository _para1Repository;

    public UpdateParameterCommandHandler(IPara1Repository para1Repository)
    {
        _para1Repository = para1Repository;
    }

    public async Task<ParameterDto> Handle(UpdateParameterCommand request, CancellationToken cancellationToken)
    {
        var para1 = await _para1Repository.GetByStampAsync(request.ParaStamp, cancellationToken)
            ?? throw new KeyNotFoundException($"Parameter with stamp {request.ParaStamp} not found");

        para1.UpdateEntity(request.Dto, request.AtualizadoPor);

        await _para1Repository.UpdateAsync(para1, cancellationToken);

        return para1.ToDto<ParameterDto>();
    }
}
