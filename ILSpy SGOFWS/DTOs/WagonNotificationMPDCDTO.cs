using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class WagonNotificationMPDCDTO
{
	public string wagonNumber { get; set; }

	public string wagonStatus { get; set; }

	public decimal? weight { get; set; }

	public string cargo { get; set; }

	public int id { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
