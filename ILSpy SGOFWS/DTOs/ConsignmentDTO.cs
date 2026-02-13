using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ConsignmentDTO
{
	public string ConsignmentNumber { get; set; }

	public List<WagonDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
