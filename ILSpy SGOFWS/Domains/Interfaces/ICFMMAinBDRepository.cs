using System;
using System.Collections.Generic;
using SGOFWS.Domains.Models.CfmMain;

namespace SGOFWS.Domains.Interfaces;

public interface ICFMMAinBDRepository
{
	List<GuiaMercadoria> GetGuiasNaoSincronizadas(DateTime dataobra, List<decimal> ndos);

	List<UBovg> GetVagoesGuiaByGuiaStamP(string stamp);
}
