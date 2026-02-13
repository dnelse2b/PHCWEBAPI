using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class KalipsoDTO
{
	public string version { get; set; }

	public string owner { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
