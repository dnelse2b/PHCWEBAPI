using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class SyncRevisaoMaterialDTO
{
	public object u_notasrev { get; set; }

	public object u_fer085 { get; set; }

	public object u_fer084 { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
