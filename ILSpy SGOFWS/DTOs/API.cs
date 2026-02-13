using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class API
{
	public string entity { get; set; }

	public string message { get; set; }

	public string status { get; set; }

	public List<Endpoint> endpoints { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
