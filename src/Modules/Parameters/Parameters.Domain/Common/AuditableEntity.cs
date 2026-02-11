namespace Parameters.Domain.Common;


public abstract class AuditableEntity
{
    public DateTime OUsrData { get; protected set; }

   
    public string OUsrHora { get; protected set; } = string.Empty;

 
    public string? OUsrInis { get; protected set; }

    public DateTime? UsrData { get; protected set; }


    public string? UsrHora { get; protected set; }


    public string? UsrInis { get; protected set; }=string.Empty;


    protected void SetCreatedAudit(string? userName = null)
    {
        var now = DateTime.Now;
        OUsrData = now.Date;
        OUsrHora = now.ToString("HH:mm:ss");
        OUsrInis = userName;
    }

  
    protected void SetUpdatedAudit(string? userName = null)
    {
        var now = DateTime.Now;
        UsrData = now.Date;
        UsrHora = now.ToString("HH:mm:ss");
        UsrInis = userName;
    }
}
