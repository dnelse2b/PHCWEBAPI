namespace Shared.Abstractions.Entities;


public abstract class AuditableEntity
{
  
    public DateTime OUsrData { get; set; }


    public string OUsrHora { get; set; } = string.Empty;

 
    public string? OUsrInis { get; set; }

    public DateTime? UsrData { get; set; }

 
    public string? UsrHora { get; set; }

  
    public string? UsrInis { get; set; }

    protected void SetCreatedAudit(string? createdBy)
    {
        var now = DateTime.UtcNow;
        OUsrData = now;
        OUsrHora = now.ToString("HH:mm:ss");
        OUsrInis = createdBy;
    }

   
    protected void SetUpdatedAudit(string? updatedBy)
    {
        var now = DateTime.UtcNow;
        UsrData = now;
        UsrHora = now.ToString("HH:mm:ss");
        UsrInis = updatedBy;
    }
}
