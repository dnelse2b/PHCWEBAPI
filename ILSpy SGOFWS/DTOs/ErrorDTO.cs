using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ErrorDTO
{
	public string message { get; set; }

	public string stack { get; set; }

	public string inner { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
