namespace Parameters.Application.Features.GetParameterByStamp;

public record ParameterDto
{
    public string ParaStamp { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int? Dec { get; init; }
    public int? Tam { get; init; }
    public DateTime OUsrData { get; init; }
    public string OUsrHora { get; init; } = string.Empty;
    public string? OUsrInis { get; init; }
    public DateTime? UsrData { get; init; }
    public string? UsrHora { get; init; }
    public string? UsrInis { get; init; }
}
