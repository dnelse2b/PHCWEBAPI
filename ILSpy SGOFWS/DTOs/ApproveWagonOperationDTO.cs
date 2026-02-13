using System;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ApproveWagonOperationDTO
{
	public string requestNumber { get; set; }

	public string requestType { get; set; }

	public bool autoShuntingStartEnd { get; set; }

	public DateTime autoShuntingStartDate { get; set; } = new DateTime(1900, 1, 1);

	public DateTime autoShuntingEndDate { get; set; } = new DateTime(1900, 1, 1);

	public string shuntingDate { get; set; }

	public string shuntingTime { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
