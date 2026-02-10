using MediatR;
using Parameters.Application.DTOs;

namespace Parameters.Application.Commands;

/// <summary>
/// Command para criar um novo Parâmetro
/// </summary>
public record CreateParameterCommand(
    string Code,
    string Description,
    CreateE4Dto? E4Details,
    string? CreatedBy = null
) : IRequest<ParameterDto>;
