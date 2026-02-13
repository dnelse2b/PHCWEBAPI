using System.Collections.Generic;

namespace SGOFWS.Domains.Models.CfmMain;

public class GuiaMercadoria
{
	public Bo bo { get; set; }

	public Bo2 bo2 { get; set; }

	public Bo3 bo3 { get; set; }

	public List<UBovg> vagoes { get; set; }
}
