using System.Collections.Generic;
using SGOFWS.Domains.Models;

namespace SGOFWS.DTOs;

public class ComposicaoMapeadaDTO
{
	public List<Composicao> composicoes { get; set; }

	public List<LinhaComposicao> linhasComposicao { get; set; }

	public List<UNotascomp> notasComp { get; set; }
}
