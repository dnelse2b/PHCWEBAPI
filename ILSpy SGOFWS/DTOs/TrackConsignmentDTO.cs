using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class TrackConsignmentDTO
{
	public string trackIn { get; set; } = "";

	public string destinationCode { get; set; } = "";

	public string destination { get; set; } = "";

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
