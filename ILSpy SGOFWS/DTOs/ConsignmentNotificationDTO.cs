using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ConsignmentNotificationDTO
{
	public string consignmentNumber { get; set; }

	public string destination { get; set; } = "Port";

	public string countryCode { get; set; }

	public List<WagonNotificationMPDCDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
