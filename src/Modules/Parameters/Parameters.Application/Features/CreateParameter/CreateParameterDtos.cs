namespace Parameters.Application.Features.CreateParameter;

/// <summary>
/// DTO para criação de E4
/// </summary>
public record CreateE4Dto
{
    public string Value { get; init; } = string.Empty;
    public string? AdditionalInfo { get; init; }
    public int Sequence { get; init; }
}

/// <summary>
/// DTO de resposta para Parâmetro
/// </summary>
public record ParameterDto
{
    public string E1Stamp { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public E4Dto? E4 { get; init; }
}

/// <summary>
/// DTO para E4
/// </summary>
public record E4Dto
{
    public string E4Stamp { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? AdditionalInfo { get; init; }
    public int Sequence { get; init; }
}
