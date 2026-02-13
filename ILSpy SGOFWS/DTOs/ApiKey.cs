using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ApiKey
{
	public string empresa { get; set; }

	public string key { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
