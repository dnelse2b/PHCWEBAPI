using Newtonsoft.Json;

namespace SGOFWS.Persistence.DTOS;

public class UpdateStationWagonDTO
{
	public string wagonNumber { get; set; }

	public string consignmentNumber { get; set; }

	public string wagonStatus { get; set; }

	public decimal? weight { get; set; }

	public string cargo { get; set; } = "";

	public bool? detached { get; set; } = false;

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
