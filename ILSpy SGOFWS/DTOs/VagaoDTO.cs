using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class VagaoDTO
{
	public decimal? UPeso { get; set; }

	public string UStatus { get; set; }

	public string UUltdtrep { get; set; }

	public string UUltmtmst { get; set; }

	public string UVagcod { get; set; }

	public string UAdmintr { get; set; }

	public string UEta { get; set; }

	public string UVagdesc { get; set; }

	public string UVagno { get; set; }

	public string UVagtip { get; set; }

	public decimal? UVagvol { get; set; }

	public string Consgno { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
