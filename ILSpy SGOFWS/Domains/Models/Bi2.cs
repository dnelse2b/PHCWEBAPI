using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class Bi2
{
	public string Bi2stamp { get; set; } = "";

	public string Bostamp { get; set; } = "";

	public string Ousrinis { get; set; } = "";

	public DateTime? Ousrdata { get; set; } = DateTime.Now;

	public string Ousrhora { get; set; } = "";

	public string Usrinis { get; set; } = "";

	public DateTime? Usrdata { get; set; } = DateTime.Now;

	public string Usrhora { get; set; } = "";

	public string UCntdesc { get; set; } = "";

	public string UProxst { get; set; } = "";

	public string UCodactst { get; set; } = "";

	public string UComboio { get; set; } = "";

	public string UContdcod { get; set; } = "";

	public string UDatcarrg { get; set; } = "";

	public string UDsccmcag { get; set; } = "";

	public string UEncerrno { get; set; } = "";

	public string UEncrradm { get; set; } = "";

	public string UHorcarrg { get; set; } = "";

	public decimal? UPesagent { get; set; }

	public decimal? UPeso { get; set; }

	public string UStatus { get; set; } = "";

	public string UUltdtrep { get; set; } = "";

	public DateTime? UDtcheg { get; set; }

	public DateTime? UDtop { get; set; }

	public string UUltmtmst { get; set; } = "";

	public string UVagcod { get; set; } = "";

	public string UAdmintr { get; set; } = "";

	public string UEta { get; set; } = "";

	public string UEtafrt { get; set; } = "";

	public string UVagdesc { get; set; } = "";

	public string UVagno { get; set; } = "";

	public string UVagtip { get; set; } = "";

	public string UEtanot { get; set; } = "";

	public string UEtaini { get; set; } = "";

	public decimal? UVagvol { get; set; }

	public bool? UDualcon { get; set; }

	public decimal? UTotcont { get; set; }

	public string UStact { get; set; } = "";

	public bool? UFullempy { get; set; }

	public bool? UChegfrt { get; set; }

	public string UAgnome { get; set; } = "";

	public string UAgemail { get; set; } = "";

	public string UAgtelef { get; set; } = "";

	public string UAgnuit { get; set; } = "";

	public string UEstadorv { get; set; } = "";

	public bool? URevisto { get; set; } = false;

	public List<UBocont> contentores { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
