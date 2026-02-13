using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class MPDCAuthResponse
{
	public string customerName { get; set; }

	public string fullName { get; set; }

	public string token { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
