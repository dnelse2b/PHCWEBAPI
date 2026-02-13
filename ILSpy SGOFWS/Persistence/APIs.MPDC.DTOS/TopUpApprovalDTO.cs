using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class TopUpApprovalDTO
{
	public decimal wagonsSupplyId { get; set; }

	public DateTime approvedAt { get; set; }

	public string Bulletin { get; set; }

	public List<string> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
