namespace Parameters.Application.DTOs;

/// <summary>
/// DTO para criação de Parâmetro
/// </summary>
public record CreateParameterDto
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CreateE4Dto? E4Details { get; init; }
}

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
/// DTO para atualização de Parâmetro
/// </summary>
public record UpdateParameterDto
{
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public UpdateE4Dto? E4Details { get; init; }
}

/// <summary>
/// DTO para atualização de E4
/// </summary>
public record UpdateE4Dto
{
    public string Value { get; init; } = string.Empty;
    public string? AdditionalInfo { get; init; }
    public int Sequence { get; init; }
}
