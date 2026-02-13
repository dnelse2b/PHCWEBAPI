using System.Collections.Generic;
using Newtonsoft.Json;
using SGOFWS.DTOs;

namespace SGOFWS.Domains.Models;

public class Dossier
{
	public Bo bo { get; set; }

	public Bo2 bo2 { get; set; }

	public Bo3 bo3 { get; set; }

	public string obs { get; set; }

	public ResponseCodesDTO processamentoResponse { get; set; }

	public List<Bi> bi { get; set; }

	public List<Bi2> bi2 { get; set; }

	public bool? novo { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
