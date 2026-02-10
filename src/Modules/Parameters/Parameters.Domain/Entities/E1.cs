namespace Parameters.Domain.Entities;

/// <summary>
/// Entidade E1 - Parte principal da tabela de Parâmetros
/// </summary>
public class E1
{
    public string E1Stamp { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Navigation property
    public virtual E4? E4 { get; private set; }

    private E1() { }

    public E1(string e1Stamp, string code, string description, string? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(e1Stamp))
            throw new ArgumentException("E1Stamp cannot be empty", nameof(e1Stamp));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        E1Stamp = e1Stamp;
        Code = code;
        Description = description ?? string.Empty;
        Active = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(string code, string description, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        Code = code;
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;

    public void SetE4(E4 e4)
    {
        if (e4.E4Stamp != E1Stamp)
            throw new InvalidOperationException("E4Stamp must match E1Stamp");

        E4 = e4;
    }
}
