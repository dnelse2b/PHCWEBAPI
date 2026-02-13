using System;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class UBocont
{
	public string UBocontstamp { get; set; }

	public string Nocl { get; set; }

	public string customerName { get; set; }

	public string Bi2stamp { get; set; }

	public string Dtcarrg { get; set; }

	public string Hrcarrg { get; set; }

	public string Stcarrg { get; set; }

	public string Vagno { get; set; }

	public string Desvcarrg { get; set; }

	public string Bistamp { get; set; }

	public string Codstcarrg { get; set; }

	public string Coddesvcarrg { get; set; }

	public string Stdest { get; set; }

	public string Desvdest { get; set; }

	public string Codstdest { get; set; }

	public string Coddesvdest { get; set; }

	public string Expedidor { get; set; }

	public string Expedidorno { get; set; }

	public string Consgt { get; set; }

	public string Consgtno { get; set; }

	public string Nocontent { get; set; }

	public string Contentscode { get; set; }

	public string Dsccmcarg { get; set; }

	public string Codcarg { get; set; }

	public decimal? Peso { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Bostamp { get; set; }

	public bool? Cargaperigosa { get; set; }

	public bool? Cargagranel { get; set; }

	public string Tamanho { get; set; }

	public string Categoria { get; set; }

	public string Contordno { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
