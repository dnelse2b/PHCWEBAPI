using System.Collections.Generic;
using SGOFWS.Domains.Models;

namespace SGOFWS.DTOs;

public class RevisaoMaterialMapeadaDTO
{
	public List<UNotasrev> notasRevisao { get; set; }

	public List<RevisaoMaterial> revisoesMaterial { get; set; }

	public List<LinhaRevisao> linhasRevisao { get; set; }
}
