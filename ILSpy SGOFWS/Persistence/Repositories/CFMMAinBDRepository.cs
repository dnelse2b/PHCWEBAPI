using System;
using System.Collections.Generic;
using System.Linq;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models.CfmMain;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Persistence.Repositories;

public class CFMMAinBDRepository : ICFMMAinBDRepository
{
	private readonly CFMMAINCONTEXT _cFMMAINCONTEXT;

	public CFMMAinBDRepository(CFMMAINCONTEXT cFMMAINCONTEXT)
	{
		_cFMMAINCONTEXT = cFMMAINCONTEXT;
	}

	public List<GuiaMercadoria> GetGuiasNaoSincronizadas(DateTime dataobra, List<decimal> ndos)
	{
		IQueryable<GuiaMercadoria> query = from bo in _cFMMAINCONTEXT.Bo
			join bo2 in _cFMMAINCONTEXT.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			join bo3 in _cFMMAINCONTEXT.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			where bo.Dataobra >= ((DateTime)dataobra).Date && ndos.Contains(bo.Ndos) && bo3.UGsync == false
			select new GuiaMercadoria
			{
				bo = new Bo
				{
					Bostamp = bo.Bostamp,
					Ndos = bo.Ndos,
					Obrano = bo.Obrano,
					Boano = bo.Boano,
					Dataobra = bo.Dataobra,
					Nome = bo.Nome,
					No = bo.No,
					Ousrdata = bo.Ousrdata,
					Usrdata = bo.Usrdata
				},
				bo2 = new Bo2
				{
					Bo2stamp = bo2.Bo2stamp,
					UFluxo = bo2.UFluxo
				},
				bo3 = new Bo3
				{
					Bo3stamp = bo3.Bo3stamp,
					UGsync = bo3.UGsync
				}
			};
		return query.ToList();
	}

	public List<UBovg> GetVagoesGuiaByGuiaStamP(string stamp)
	{
		return _cFMMAINCONTEXT.UBovg.Where((UBovg bvg) => bvg.Bostamp == stamp).ToList();
	}
}
