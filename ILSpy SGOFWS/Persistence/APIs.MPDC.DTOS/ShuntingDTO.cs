using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class ShuntingDTO
{
	public string wagonsRequestId { get; set; }

	public string? shuntingStartDate { get; set; }

	public string requestType { get; set; } = "";

	public string aprovedBy { get; set; } = "";

	public string operatorName { get; set; } = "";

	[JsonProperty(PropertyName = "BollentinId")]
	public string shuntingId { get; set; } = "";

	public string? shuntingEndDate { get; set; }

	public DateTime approvalDate { get; set; } = new DateTime(1900, 1, 1);

	public string approvalTime { get; set; } = "00:00";

	public List<string> wagons { get; set; } = new List<string>();
}
