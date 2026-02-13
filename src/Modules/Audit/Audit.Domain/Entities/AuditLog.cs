using Shared.Kernel.Extensions;

namespace Audit.Domain.Entities;

public class AuditLog
{
    public string ULogsstamp { get; private set; } = string.Empty;
    public string? RequestId { get; private set; }
    public DateTime? Data { get; private set; }
    public string? Code { get; private set; }
    public string? Content { get; private set; }
    public string? Ip { get; private set; }
    public string? ResponseDesc { get; private set; }
    public string? ResponseText { get; private set; }
    public string? Operation { get; private set; }

    private AuditLog() { }

    public AuditLog(
        string code,
        string? requestId,
        string responseDesc,
        string? operation,
        string? content = null,
        string? responseText = null,
        string? ip = null)
    {
        ULogsstamp = 25.GenerateStamp();
        RequestId = requestId;
        Data = DateTime.UtcNow;
        Code = code;
        Content = content;
        Ip = ip;
        ResponseDesc = responseDesc;
        ResponseText = responseText;
        Operation = operation;
    }

  
    public void UpdateResponseText(string responseText)
    {
        ResponseText = responseText;
    }
}
