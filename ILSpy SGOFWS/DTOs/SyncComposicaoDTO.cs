using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class SyncComposicaoDTO
{
	public object u_fer10055 { get; set; }

	public object u_fer1006 { get; set; }

	public object u_notascomp { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
