using System.Collections.Generic;

namespace SGOFWS.Domains.Models;

public class RotaTrack
{
	public URota rota { get; set; }

	public List<UTrajrot> trajecto { get; set; }

	public List<UEntrot> entidades { get; set; }
}
