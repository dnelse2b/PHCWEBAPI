using System;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class MineDepartureNotificationMPDCDTO
{
	public string consignmentNumber { get; set; }

	public string origin { get; set; }

	public string destination { get; set; } = "TCM";

	public DateTime departureDate { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
