namespace Parameters.Application.DTOs;

/// <summary>
/// DTO para leitura de Parâmetro
/// </summary>
public record ParameterDto
{
    public string E1Stamp { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
    public E4Dto? E4Details { get; init; }
}

/// <summary>
/// DTO para leitura de E4
/// </summary>
public record E4Dto
{
    public string E4Stamp { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? AdditionalInfo { get; init; }
    public int Sequence { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
