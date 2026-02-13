using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class UpdateConsignmentDataMPDCDTO
{
	[Required]
	public string consignmentNumber { get; set; }

	[Required]
	public string countryCode { get; set; }

	[Required]
	[StringLength(20)]
	public string customerNuit { get; set; }

	[Required]
	[StringLength(55)]
	public string customerName { get; set; }

	[StringLength(100)]
	public string customerEmail { get; set; }

	[StringLength(50)]
	public string customerPhone { get; set; }

	[Required]
	[StringLength(20)]
	public string agentNuit { get; set; }

	[Required]
	[StringLength(250)]
	public string agentName { get; set; }

	[StringLength(100)]
	public string agentEmail { get; set; }

	[StringLength(100)]
	public string agentPhone { get; set; }

	public List<WagonMPDCDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
