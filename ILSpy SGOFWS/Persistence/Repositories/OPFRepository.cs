using System.Collections.Generic;
using System.Linq;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Persistence.Repositories;

public class OPFRepository : IOPFRepository
{
	private SGOFCTX _SGOFCTX;

	public OPFRepository(SGOFCTX SGOFCTX)
	{
		_SGOFCTX = SGOFCTX;
	}

	public ComboioRegisto GetComboioRegistoByRef(string combref)
	{
		return _SGOFCTX.ComboioRegisto.Where((ComboioRegisto cr) => cr.Ref == combref).FirstOrDefault();
	}

	public List<Tempos> GetTemposByComboioStamp(string comboioStamp)
	{
		return _SGOFCTX.Tempos.Where((Tempos tmp) => tmp.ComboioRegistoStamp == comboioStamp).ToList();
	}

	public Bi2 GetBi2Bystamp(string bi2stamp)
	{
		return _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.Bi2stamp == bi2stamp).FirstOrDefault();
	}

	public Recebimento GetRecebimentoByRevisaoStamp(string revistaoStamp)
	{
		return _SGOFCTX.Recebimento.Where((Recebimento rc) => rc.Revisaostamp == revistaoStamp).FirstOrDefault();
	}

	public RevisaoMaterial GetRevisaoByStamp(string revistaoStamp)
	{
		return _SGOFCTX.RevisaoMaterial.Where((RevisaoMaterial rv) => rv.RevisaoMaterialstamp == revistaoStamp).FirstOrDefault();
	}

	public Telemetro GetTelemetroByNo(string no)
	{
		return _SGOFCTX.Telemetro.Where((Telemetro tm) => tm.No == no.Trim()).FirstOrDefault();
	}

	public Composicao GetComposicaoByStamp(string composicaoStamp)
	{
		return _SGOFCTX.Composicao.Where((Composicao cmp) => cmp.Composicaostamp == composicaoStamp).FirstOrDefault();
	}
}
