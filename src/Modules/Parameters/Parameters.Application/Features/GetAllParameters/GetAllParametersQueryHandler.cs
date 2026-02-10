using MediatR;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Features.GetAllParameters;

/// <summary>
/// Handler para obter todos os Parâmetros
/// </summary>
public class GetAllParametersQueryHandler : IRequestHandler<GetAllParametersQuery, IEnumerable<ParameterDto>>
{
    private readonly IE1Repository _e1Repository;
    private readonly IE4Repository _e4Repository;

    public GetAllParametersQueryHandler(
        IE1Repository e1Repository,
        IE4Repository e4Repository)
    {
        _e1Repository = e1Repository;
        _e4Repository = e4Repository;
    }

    public async Task<IEnumerable<ParameterDto>> Handle(GetAllParametersQuery request, CancellationToken cancellationToken)
    {
        var e1List = await _e1Repository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var e4List = await _e4Repository.GetAllAsync(cancellationToken);

        var e4Dictionary = e4List.ToDictionary(e4 => e4.E4Stamp);

        var result = e1List.Select(e1 =>
        {
            e4Dictionary.TryGetValue(e1.E1Stamp, out var e4);
            return MapToDto(e1, e4);
        });

        return result;
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
