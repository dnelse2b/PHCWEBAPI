using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class NovaConsignacaoDTOMPDC
{
	public string consignmentNumber { get; set; }

	public string consignmentID { get; set; }

	public List<WagonNotificationMPDCDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
