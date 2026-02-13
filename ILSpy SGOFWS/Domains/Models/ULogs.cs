using System;
using SGOFWS.Extensions;

namespace SGOFWS.Domains.Models;

public class ULogs
{
	public string ULogsstamp { get; set; } = 25.UseThisSizeForStamp();

	public string? RequestId { get; set; } = "";

	public DateTime? Data { get; set; } = new DateTime(1900, 1, 1);

	public string? Code { get; set; } = "";

	public string? Content { get; set; } = "";

	public string? Ip { get; set; } = "";

	public string? ResponseDesc { get; set; }

	public string? ResponseText { get; set; } = "";

	public string? Operation { get; set; } = "";
}
