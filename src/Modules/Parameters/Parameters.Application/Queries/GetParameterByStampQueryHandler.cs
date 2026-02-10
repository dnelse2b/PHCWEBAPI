using MediatR;
using Parameters.Application.DTOs;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Queries;

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
                Sequence = e4.Sequence,
                CreatedAt = e4.CreatedAt,
                UpdatedAt = e4.UpdatedAt
            };
        }

        return new ParameterDto
        {
            E1Stamp = e1.E1Stamp,
            Code = e1.Code,
            Description = e1.Description,
            Active = e1.Active,
            CreatedAt = e1.CreatedAt,
            UpdatedAt = e1.UpdatedAt,
            CreatedBy = e1.CreatedBy,
            UpdatedBy = e1.UpdatedBy,
            E4Details = e4Dto
        };
    }
}
