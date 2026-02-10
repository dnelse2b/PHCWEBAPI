namespace Parameters.Application.Features.UpdateParameter;

/// <summary>
/// DTO de entrada para atualização de Parâmetro (usado no Controller)
/// </summary>
public record UpdateParameterDto
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public UpdateE4Dto? E4Details { get; init; }
}
