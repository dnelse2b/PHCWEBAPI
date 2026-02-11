using Parameters.Domain.Common;

namespace Parameters.Domain.Entities;


public class Para1 : AuditableEntity
{
    public string ParaStamp { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public string Valor { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = string.Empty;
    public int? Dec { get; private set; }
    public int? Tam { get; private set; }

    private Para1() { }

    public Para1(string paraStamp, string descricao, string valor, string tipo, int? dec = null, int? tam = null, string? criadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(paraStamp))
            throw new ArgumentException("ParaStamp cannot be empty", nameof(paraStamp));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descricao cannot be empty", nameof(descricao));

        ParaStamp = paraStamp;
        Descricao = descricao;
        Valor = valor ?? string.Empty;
        Tipo = tipo ?? string.Empty;
        Dec = dec;
        Tam = tam;

        // Auditoria de criação
        SetCreatedAudit(criadoPor);
    }

    public void Update(string descricao, string valor, string tipo, int? dec, int? tam, string? atualizadoPor = null)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descricao cannot be empty", nameof(descricao));

        Descricao = descricao;
        Valor = valor ?? string.Empty;
        Tipo = tipo ?? string.Empty;
        Dec = dec;
        Tam = tam;

        // Auditoria de atualização
        SetUpdatedAudit(atualizadoPor);
    }
}
