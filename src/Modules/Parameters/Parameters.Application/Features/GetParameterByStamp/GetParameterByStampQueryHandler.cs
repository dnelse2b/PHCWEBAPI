using MediatR;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Features.GetParameterByStamp;

/// <summary>
/// Handler para obter Parâmetro por Stamp
/// </summary>
public class GetParameterByStampQueryHandler : IRequestHandler<GetParameterByStampQuery, ParameterDto?>
{
    private readonly IE1Repository _e1Repository;
    private readonly IE4Repository _e4Repository;

    public GetParameterByStampQueryHandler(
        IE1Repository e1Repository,
        IE4Repository e4Repository)
    {
        _e1Repository = e1Repository;
        _e4Repository = e4Repository;
    }

    public async Task<ParameterDto?> Handle(GetParameterByStampQuery request, CancellationToken cancellationToken)
    {
        var e1 = await _e1Repository.GetByStampAsync(request.E1Stamp, cancellationToken);
        if (e1 == null)
            return null;

        var e4 = await _e4Repository.GetByStampAsync(request.E1Stamp, cancellationToken);

        return MapToDto(e1, e4);
    }

    private static ParameterDto MapToDto(Domain.Entities.E1 e1, Domain.Entities.E4? e4)
    {
        E4Dto? e4Dto = null;
        if (e4 != null)
        {
            e4Dto = new E4Dto
            {
                E4Stamp = e4.E4Stamp,
                Value = e4.Value,
                AdditionalInfo = e4.AdditionalInfo,
                Sequence = e4.Sequence
            };
        }

        return new ParameterDto
        {
            E1Stamp = e1.E1Stamp,
            Code = e1.Code,
            Description = e1.Description,
            Active = e1.Active,
            CreatedBy = e1.CreatedBy,
            CreatedAt = e1.CreatedAt,
            E4 = e4Dto
        };
    }
}
