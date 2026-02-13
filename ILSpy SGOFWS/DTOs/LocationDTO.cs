using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class LocationDTO
{
	public string Code { get; set; } = "";

	public string Designation { get; set; } = "";

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
