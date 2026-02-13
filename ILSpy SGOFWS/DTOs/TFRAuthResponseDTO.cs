using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class TFRAuthResponseDTO
{
	public string bioSecretToken { get; set; }

	public string token { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
