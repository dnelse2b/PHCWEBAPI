using MediatR;
using Parameters.Domain.Repositories;
using Parameters.Application.Mappings;

namespace Parameters.Application.Features.CreateParameter;

public class CreateParameterCommandHandler : IRequestHandler<CreateParameterCommand, ParameterDto>
{
    private readonly IPara1Repository _para1Repository;

    public CreateParameterCommandHandler(IPara1Repository para1Repository)
    {
        _para1Repository = para1Repository;
    }

    public async Task<ParameterDto> Handle(CreateParameterCommand request, CancellationToken cancellationToken)
    {
        var para1 = request.Dto.ToEntity(request.CriadoPor);

        var savedPara1 = await _para1Repository.AddAsync(para1, cancellationToken);

        return savedPara1.ToDto<ParameterDto>();
    }
}
