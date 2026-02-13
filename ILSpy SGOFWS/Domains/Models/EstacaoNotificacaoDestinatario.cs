using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class EstacaoNotificacaoDestinatario
{
	public UEstacnt estacnt { get; set; }

	public UDestnot destnot { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
