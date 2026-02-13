using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class WagonRequestResultDTO
{
	public string wagonNumber { get; set; }

	public string consignmentNumber { get; set; }

	public bool processed { get; set; }

	public string errorMessage { get; set; } = "";

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
