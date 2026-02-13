using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class Credentials
{
	public string username { get; set; }

	public string password { get; set; }

	public string apiKey { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
