using System;

namespace SGOFWS.Domains.Models;

public class UStqueue
{
	public string UStqueuestamp { get; set; }

	public string Train { get; set; }

	public string Operationtype { get; set; }

	public DateTime Operationdate { get; set; }

	public string Station { get; set; }

	public string Tempostamp { get; set; }

	public string Nextstation { get; set; }

	public DateTime Etanextst { get; set; }

	public DateTime Etabossa { get; set; }

	public DateTime Etadestination { get; set; }

	public string Stamptrainreg { get; set; }

	public string Trainorientation { get; set; }

	public string Destination { get; set; }

	public string Coddest { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public string Trainnumber { get; set; }

	public string Trainreference { get; set; }

	public string Wagonsdata { get; set; }

	public string Entity { get; set; }

	public decimal Ord { get; set; }

	public string Stationcode { get; set; }

	public string Nextstcode { get; set; }
}
