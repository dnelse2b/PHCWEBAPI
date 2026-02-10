namespace Parameters.Application.Features.CreateParameter;

/// <summary>
/// DTO de entrada para criação de Parâmetro (usado no Controller)
/// </summary>
public record CreateParameterDto
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CreateE4Dto? E4Details { get; init; }
}
