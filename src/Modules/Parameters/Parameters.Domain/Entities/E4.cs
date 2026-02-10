namespace Parameters.Domain.Entities;

/// <summary>
/// Entidade E4 - Parte complementar da tabela de Parâmetros
/// </summary>
public class E4
{
    public string E4Stamp { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? AdditionalInfo { get; private set; }
    public int Sequence { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation property
    public virtual E1? E1 { get; private set; }

    private E4() { }

    public E4(string e4Stamp, string value, int sequence = 0, string? additionalInfo = null)
    {
        if (string.IsNullOrWhiteSpace(e4Stamp))
            throw new ArgumentException("E4Stamp cannot be empty", nameof(e4Stamp));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        E4Stamp = e4Stamp;
        Value = value;
        Sequence = sequence;
        AdditionalInfo = additionalInfo;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string value, int sequence, string? additionalInfo = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        Value = value;
        Sequence = sequence;
        AdditionalInfo = additionalInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetE1(E1 e1)
    {
        if (e1.E1Stamp != E4Stamp)
            throw new InvalidOperationException("E1Stamp must match E4Stamp");

        E1 = e1;
    }
}
