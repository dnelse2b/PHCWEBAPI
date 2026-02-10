using MediatR;
using Parameters.Domain.Entities;
using Parameters.Domain.Repositories;

namespace Parameters.Application.Features.CreateParameter;

/// <summary>
/// Handler para criar Parâmetro
/// </summary>
public class CreateParameterCommandHandler : IRequestHandler<CreateParameterCommand, ParameterDto>
{
    private readonly IE1Repository _e1Repository;
    private readonly IE4Repository _e4Repository;

    public CreateParameterCommandHandler(
        IE1Repository e1Repository,
        IE4Repository e4Repository)
    {
        _e1Repository = e1Repository;
        _e4Repository = e4Repository;
    }

    public async Task<ParameterDto> Handle(CreateParameterCommand request, CancellationToken cancellationToken)
    {
        // Generate unique stamp
        var stamp = GenerateStamp();

        // Create E1 entity
        var e1 = new E1(
            stamp,
            request.Code,
            request.Description,
            request.CreatedBy
        );

        // Create E4 entity if provided
        E4? e4 = null;
        if (request.E4Details != null)
        {
            e4 = new E4(
                stamp, // Same stamp as E1
                request.E4Details.Value,
                request.E4Details.Sequence,
                request.E4Details.AdditionalInfo
            );

            e1.SetE4(e4);
        }

        // Save to database
        var savedE1 = await _e1Repository.AddAsync(e1, cancellationToken);

        if (e4 != null)
        {
            await _e4Repository.AddAsync(e4, cancellationToken);
        }

        // Map to DTO
        return MapToDto(savedE1, e4);
    }

    private static string GenerateStamp()
    {
        // Generate unique stamp following enterprise pattern
        // Format: YYYYMMDDHHMMSS + Random
        return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString("N")[..8].ToUpper();
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
