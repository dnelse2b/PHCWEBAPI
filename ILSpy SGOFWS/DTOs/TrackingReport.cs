using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SGOFWS.Domains.Models;

namespace SGOFWS.DTOs;

public class TrackingReport
{
	public List<Dossier> novasConsignacoes { get; set; }

	public List<VagaoDTO> vagoesActualizados { get; set; }

	public decimal? totalNovasConsignacoes { get; set; }

	public DateTime? data { get; set; }

	public decimal? totalVagoesActualizados { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
