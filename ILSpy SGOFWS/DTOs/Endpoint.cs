using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class Endpoint
{
	public string operationCode { get; set; }

	public string method { get; set; }

	public string url { get; set; }

	public string contentType { get; set; }

	public Credentials credentials { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
