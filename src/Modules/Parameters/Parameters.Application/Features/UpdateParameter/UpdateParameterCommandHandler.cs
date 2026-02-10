using MediatR;
using Parameters.Domain.Entities;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Features.UpdateParameter;

/// <summary>
/// Handler para atualizar Parâmetro
/// </summary>
public class UpdateParameterCommandHandler : IRequestHandler<UpdateParameterCommand, ParameterDto>
{
    private readonly IE1Repository _e1Repository;
    private readonly IE4Repository _e4Repository;

    public UpdateParameterCommandHandler(
        IE1Repository e1Repository,
        IE4Repository e4Repository)
    {
        _e1Repository = e1Repository;
        _e4Repository = e4Repository;
    }

    public async Task<ParameterDto> Handle(UpdateParameterCommand request, CancellationToken cancellationToken)
    {
        // Get existing E1
        var e1 = await _e1Repository.GetByStampAsync(request.E1Stamp, cancellationToken)
            ?? throw new KeyNotFoundException($"Parameter with stamp {request.E1Stamp} not found");

        // Update E1
        e1.Update(request.Code, request.Description, request.UpdatedBy);

        if (request.Active)
            e1.Activate();
        else
            e1.Deactivate();

        await _e1Repository.UpdateAsync(e1, cancellationToken);

        // Update E4 if provided
        E4? e4 = null;
        if (request.E4Details != null)
        {
            e4 = await _e4Repository.GetByStampAsync(request.E1Stamp, cancellationToken);

            if (e4 == null)
            {
                e4 = new E4(
                    request.E1Stamp,
                    request.E4Details.Value,
                    request.E4Details.Sequence,
                    request.E4Details.AdditionalInfo
                );
                await _e4Repository.AddAsync(e4, cancellationToken);
            }
            else
            {
                e4.Update(
                    request.E4Details.Value,
                    request.E4Details.Sequence,
                    request.E4Details.AdditionalInfo
                );
                await _e4Repository.UpdateAsync(e4, cancellationToken);
            }
        }

        return MapToDto(e1, e4);
    }

    private static ParameterDto MapToDto(E1 e1, E4? e4)
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
