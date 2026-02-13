using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class SyncDTO
{
	public object data { get; set; }

	public string processCode { get; set; }

	public string processID { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
