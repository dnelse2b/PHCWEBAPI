namespace Parameters.Application.Features.CreateParameter;


public record CreateParameterDto
{
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
}
