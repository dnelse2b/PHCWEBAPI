using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OPPWS.Extensions;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Persistence.APIs.MPDC.DTOS;
using SGOFWS.Persistence.Contexts;
using Z.EntityFramework.Plus;

namespace SGOFWS.Persistence.Repositories;

public class ConsignacaoRepository : IConsignacaoRepository
{
	private readonly SGOFCTX _SGOFCTX;

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	public ConsignacaoRepository(SGOFCTX sGOFCTX)
	{
		_SGOFCTX = sGOFCTX;
	}

	public List<LinhasPlanoManobra> GetLinhasPlanoManobrasByPlanoStamp(string stamp)
	{
		return _SGOFCTX.LinhasPlanoManobra.Where((LinhasPlanoManobra lp) => lp.CabecalhoPlanoManobraStamp == stamp).ToList();
	}

	public List<EntidadeVagao> GetEntidadesByStampRetirada(string stamp)
	{
		return _SGOFCTX.EntidadeVagao.Where((EntidadeVagao ev) => ev.Stamppedret == stamp).ToList();
	}

	public List<VeiculosPorRegularizar> GetVeiculosPorRegularizarByPedidoStamp(string stampPedido)
	{
		return _SGOFCTX.VeiculosPorRegularizar.Where((VeiculosPorRegularizar vr) => vr.Stamppedido == stampPedido).ToList();
	}

	public List<ComboioRegisto> GetComboiosByRangeDate(DateTime startDate, DateTime dataFim)
	{
		return _SGOFCTX.ComboioRegisto.Where((ComboioRegisto cr) => cr.Data >= startDate && cr.Data <= dataFim).ToList();
	}

	public List<ManobraELinha> GetManobrasByRange(DateTime dataInicio, DateTime dataFim)
	{
		IQueryable<ManobraELinha> query = from manobra in _SGOFCTX.Manobras
			join linhasManobra in _SGOFCTX.LinhasManobra on manobra.ManobraStamp equals linhasManobra.ManobraStamp
			where manobra.Dataini >= dataInicio && manobra.Dataini <= dataFim && manobra.Pedidoid.Length > 0
			select new ManobraELinha
			{
				linhaManobra = linhasManobra,
				manobra = manobra
			};
		return query.ToList();
	}

	public LinhasManobra GetLinhaManobraByStampManobraAndVeiculo(string stampManobra, string novg)
	{
		return _SGOFCTX.LinhasManobra.Where((LinhasManobra lm) => lm.ManobraStamp == stampManobra && lm.Novg == novg).FirstOrDefault();
	}

	public List<LinhasManobra> GetLinhasManobrasByManobraStamp(string stamp)
	{
		return _SGOFCTX.LinhasManobra.Where((LinhasManobra lm) => lm.ManobraStamp == stamp).ToList();
	}

	public ComboioRegisto GetComboioVagao(string entidadeVagaoStamp)
	{
		IQueryable<ComboioRegisto> query = from entidadeVagao in _SGOFCTX.EntidadeVagao
			join linhaVeiculo in _SGOFCTX.LinhaVeiculoFerroviario on entidadeVagao.EntidadeVagaostamp equals linhaVeiculo.Stampevag
			join comboio in _SGOFCTX.ComboioRegisto on linhaVeiculo.Stampcomboio equals comboio.ComboioRegistoStamp
			where entidadeVagao.EntidadeVagaostamp == entidadeVagaoStamp && entidadeVagao.Fechado == (bool?)false && comboio.Chegou == (bool?)true
			select new ComboioRegisto();
		return query.FirstOrDefault();
	}

	public Cl GetClienteByNome(string nome)
	{
		return _SGOFCTX.Cl.Where((Cl cl) => cl.Nome.Trim() == nome.Trim()).FirstOrDefault();
	}

	public decimal? getMaxObrano(decimal? ndos)
	{
		return (from bo in _SGOFCTX.Bo
			where bo.Ndos == ndos && bo.Boano == (decimal?)(decimal)DateTime.Now.Year
			select bo.Obrano).ToList().DefaultIfEmpty(default(decimal)).Max() + (decimal?)1;
	}

	public List<UStqueue> GetStQueueByEntity(string entity)
	{
		return _SGOFCTX.UStqueue.Where((UStqueue st) => st.Entity.Trim() == entity).ToList();
	}

	public void criarConsignacao(Dossier dossier)
	{
		_SGOFCTX.ChangeTracker.Clear();
		DateTime dataobra = DateTime.Now;
		int boano = DateTime.Now.Year;
		string consignacaostamp = "OFWS" + 21.UseThisSizeForStamp();
		Bo bo = dossier.bo;
		Bo2 bo2 = dossier.bo2;
		Bo3 bo3 = dossier.bo3;
		List<Bi> bis = dossier.bi;
		List<Bi2> bi2s = dossier.bi2;
		decimal? ndos = bo.Ndos;
		decimal? obrano = bo.Obrano;
		string nmdos = bo.Nmdos;
		bo.Bostamp = consignacaostamp;
		bo.Boano = boano;
		bo.Dataobra = dataobra;
		bo2.Bo2stamp = consignacaostamp;
		bo3.Bo3stamp = consignacaostamp;
		bo.Ousrdata = dataobra;
		bo.Usrdata = dataobra;
		bo2.Usrdata = dataobra;
		bo3.Ousrdata = dataobra;
		bo3.Usrdata = dataobra;
		bo.AssignDefaultValues();
		bo2.AssignDefaultValues();
		bo3.AssignDefaultValues();
		_SGOFCTX.Entry(bo).State = EntityState.Added;
		_SGOFCTX.Entry(bo2).State = EntityState.Added;
		_SGOFCTX.Entry(bo3).State = EntityState.Added;
		inserirVagoes(bis, bi2s, bo);
		_SGOFCTX.SaveChanges();
	}

	public void inserirVagoes(List<Bi> bis, List<Bi2> bi2s, Bo consignacao)
	{
		int lordem = 1;
		for (int i = 0; i < bi2s.Count; i++)
		{
			string vagaostamp = "OFWS" + 21.UseThisSizeForStamp();
			bis[i].Bistamp = vagaostamp;
			bis[i].Bostamp = consignacao.Bostamp;
			bis[i].Ndos = consignacao.Ndos;
			bis[i].Nmdos = consignacao.Nmdos;
			bis[i].Obrano = consignacao.Obrano;
			bis[i].Dataobra = consignacao.Dataobra;
			bis[i].No = consignacao.No;
			bis[i].Nome = consignacao.Nome;
			bis[i].Lordem = lordem * 1000;
			bis[i].Ousrdata = consignacao.Ousrdata;
			bis[i].Usrdata = consignacao.Usrdata;
			bi2s[i].Bi2stamp = vagaostamp;
			bi2s[i].Bostamp = consignacao.Bostamp;
			bi2s[i].UEtaini = bi2s[i].UEta;
			bi2s[i].UEtanot = bi2s[i].UEta;
			bis[i].Ousrdata = consignacao.Ousrdata;
			bi2s[i].Usrdata = consignacao.Usrdata;
			bis[i].AssignDefaultValues();
			bi2s[i].AssignDefaultValues();
			_SGOFCTX.Entry(bi2s[i]).State = EntityState.Added;
			_SGOFCTX.Entry(bis[i]).State = EntityState.Added;
			List<UBocont> contentores = bi2s[i].contentores;
			if (contentores != null && contentores.Count > 0)
			{
				gravarContentores(contentores, vagaostamp, consignacao.Bostamp);
			}
			lordem++;
		}
		_SGOFCTX.SaveChanges();
	}

	public void deleteTest()
	{
	}

	public int RemoveLinhasManobra(string manobrastamp)
	{
		return _SGOFCTX.LinhasManobra.Where((LinhasManobra linha) => linha.ManobraStamp == manobrastamp).Delete();
	}

	public LocaisFerroviarios GetLocalFerroviario(string codigo)
	{
		return _SGOFCTX.LocaisFerroviarios.Where((LocaisFerroviarios local) => local.Codigo == codigo).FirstOrDefault();
	}

	public void gravarContentores(List<UBocont> contentores, string vagaostamp, string consignacaostamp)
	{
		foreach (UBocont contentor in contentores)
		{
			string contentorstamp = "OFWS" + 21.UseThisSizeForStamp();
			contentor.Bistamp = vagaostamp;
			contentor.UBocontstamp = contentorstamp;
			contentor.Bostamp = consignacaostamp;
			Contentor cont = new Contentor
			{
				UBocontstamp = contentorstamp,
				Nocl = contentor.Nocl,
				customerName = contentor.customerName,
				Dtcarrg = contentor.Dtcarrg,
				Hrcarrg = contentor.Hrcarrg,
				Stcarrg = contentor.Stcarrg,
				Desvcarrg = contentor.Desvcarrg,
				Bostamp = consignacaostamp,
				Nocontent = contentor.Nocontent,
				Bistamp = vagaostamp,
				Codstcarrg = contentor.Codstcarrg,
				Coddesvcarrg = contentor.Coddesvcarrg,
				Stdest = contentor.Stdest,
				Desvdest = contentor.Desvdest,
				Codstdest = contentor.Codstdest,
				Coddesvdest = contentor.Coddesvdest,
				Expedidor = contentor.Expedidor,
				Expedidorno = contentor.Expedidorno,
				Consgt = contentor.Consgt,
				Contentscode = contentor.Contentscode,
				Peso = contentor.Peso,
				Dsccmcarg = contentor.Dsccmcarg,
				Consgtno = contentor.Consgtno,
				Codcarg = contentor.Codcarg,
				Cargaperigosa = contentor.Cargaperigosa,
				Tamanho = contentor.Tamanho,
				Contordno = contentor.Contordno,
				Vagno = contentor.Vagno,
				Cargagranel = contentor.Cargagranel,
				Categoria = contentor.Categoria
			};
			cont.AssignDefaultValues();
			_SGOFCTX.Entry(cont).State = EntityState.Added;
		}
	}

	public Fluxo GetFluxoByCodigo(string codigo)
	{
		return _SGOFCTX.Fluxo.Where((Fluxo fluxo) => fluxo.Codigo.Trim() == codigo).FirstOrDefault();
	}

	public void actualizarHistoricoConsignacao(string obs, string consignacaostamp)
	{
		Bo3 consgExiste = _SGOFCTX.Bo3.Where((Bo3 bo3) => bo3.Bo3stamp == consignacaostamp).FirstOrDefault();
		if (consgExiste != null)
		{
			consgExiste.UObs = consgExiste.UObs + "," + obs;
		}
		_SGOFCTX.SaveChanges();
	}

	public Dossier GetConsignacaoCabecalho(string consigno)
	{
		IQueryable<Dossier> query = from bo in _SGOFCTX.Bo
			join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			where bo3.UConsgno == consigno
			select new Dossier
			{
				bo = new Bo
				{
					Bostamp = bo.Bostamp,
					Ndos = bo.Ndos,
					Boano = bo.Boano,
					Dataobra = bo.Dataobra,
					Ousrdata = bo.Ousrdata,
					Usrdata = bo.Usrdata
				},
				bo2 = new Bo2
				{
					Bo2stamp = bo2.Bo2stamp
				},
				bo3 = new Bo3
				{
					Bo3stamp = bo3.Bo3stamp,
					UConsgno = bo3.UConsgno,
					UCoddesvg = bo3.UCoddesvg,
					UCoddesvt = bo3.UCoddesvt,
					UCodstcag = bo3.UCodstcag,
					UCodstdet = bo3.UCodstdet,
					UConsgt = bo3.UConsgt,
					UConsgtip = bo3.UConsgtip,
					UConsgtno = bo3.UConsgtno,
					UDesvcarg = bo3.UDesvcarg,
					UDesvdest = bo3.UDesvdest,
					UExped = bo3.UExped,
					UExpedno = bo3.UExpedno,
					UStcarrg = bo3.UStcarrg,
					UStkdno = bo3.UStkdno,
					UStkdnome = bo3.UStkdnome,
					UCliente = bo3.UCliente,
					UNocl = bo3.UNocl,
					UStdest = bo3.UStdest,
					UAgnome = bo3.UAgnome,
					UAgemail = bo3.UAgemail,
					UAgtelef = bo3.UAgtelef,
					UAgnuit = bo3.UAgnuit,
					UStatus = bo3.UStatus,
					UFluxocom = bo3.UFluxocom,
					UFluxotec = bo3.UFluxotec,
					UAdmin = bo3.UAdmin,
					UObs = bo3.UObs,
					UTotveicl = bo3.UTotveicl
				}
			};
		return query.FirstOrDefault();
	}

	public Dossier getConsignacao(string consigno)
	{
		IQueryable<Dossier> query = from bo in _SGOFCTX.Bo
			join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			where bo3.UConsgno == consigno
			select new Dossier
			{
				bo = new Bo
				{
					Bostamp = bo.Bostamp,
					Ndos = bo.Ndos,
					Boano = bo.Boano,
					Dataobra = bo.Dataobra,
					Ousrdata = bo.Ousrdata,
					Usrdata = bo.Usrdata
				},
				bo2 = new Bo2
				{
					Bo2stamp = bo2.Bo2stamp
				},
				bo3 = new Bo3
				{
					Bo3stamp = bo3.Bo3stamp,
					UConsgno = bo3.UConsgno,
					UCoddesvg = bo3.UCoddesvg,
					UCoddesvt = bo3.UCoddesvt,
					UCodstcag = bo3.UCodstcag,
					UCodstdet = bo3.UCodstdet,
					UConsgt = bo3.UConsgt,
					UConsgtip = bo3.UConsgtip,
					UConsgtno = bo3.UConsgtno,
					UDesvcarg = bo3.UDesvcarg,
					UDesvdest = bo3.UDesvdest,
					UExped = bo3.UExped,
					UExpedno = bo3.UExpedno,
					UStcarrg = bo3.UStcarrg,
					UStkdno = bo3.UStkdno,
					UStkdnome = bo3.UStkdnome,
					UCliente = bo3.UCliente,
					UNocl = bo3.UNocl,
					UStdest = bo3.UStdest,
					UAgnome = bo3.UAgnome,
					UAgemail = bo3.UAgemail,
					UAgtelef = bo3.UAgtelef,
					UAgnuit = bo3.UAgnuit,
					UStatus = bo3.UStatus,
					UFluxocom = bo3.UFluxocom,
					UFluxotec = bo3.UFluxotec,
					UAdmin = bo3.UAdmin,
					UObs = bo3.UObs,
					UTotveicl = bo3.UTotveicl
				}
			};
		Dossier dossierResult = query.FirstOrDefault();
		if (dossierResult == null)
		{
			return null;
		}
		dossierResult.bi2 = (from bi2 in _SGOFCTX.Bi2
			where bi2.Bostamp == dossierResult.bo.Bostamp
			select new Bi2
			{
				UCntdesc = bi2.UCntdesc,
				UCodactst = bi2.UCodactst,
				UComboio = bi2.UComboio,
				UContdcod = bi2.UContdcod,
				UDatcarrg = bi2.UDatcarrg,
				UDsccmcag = bi2.UDsccmcag,
				UEncerrno = bi2.UEncerrno,
				UEncrradm = bi2.UEncrradm,
				UHorcarrg = bi2.UHorcarrg,
				UPeso = bi2.UPeso,
				UStatus = bi2.UStatus,
				UUltdtrep = bi2.UUltdtrep,
				UUltmtmst = bi2.UUltmtmst,
				UVagcod = bi2.UVagcod,
				UAdmintr = bi2.UAdmintr,
				UEta = bi2.UEta,
				UVagdesc = bi2.UVagdesc,
				UVagno = bi2.UVagno,
				UVagtip = bi2.UVagtip,
				UEtanot = bi2.UEtanot,
				UEtaini = bi2.UEtaini,
				UVagvol = bi2.UVagvol,
				UDualcon = bi2.UDualcon,
				UTotcont = bi2.UTotcont,
				UStact = bi2.UStact,
				UFullempy = bi2.UFullempy,
				UAgnome = bi2.UAgnome,
				UAgemail = bi2.UAgemail,
				UAgtelef = bi2.UAgtelef,
				UAgnuit = bi2.UAgnuit
			}).ToList();
		dossierResult.bi = (from bi in _SGOFCTX.Bi
			where bi.Bostamp == dossierResult.bo.Bostamp
			select new Bi
			{
				Bistamp = bi.Bistamp
			}).ToList();
		return dossierResult;
	}

	public Bi2 getVagaoActualizado(string consignacaostamp, string vagno, string uUltdtrep, string status)
	{
		try
		{
			return _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.Bostamp == consignacaostamp && bi2.UVagno == vagno && (bi2.UUltdtrep != uUltdtrep || bi2.UStatus != status)).FirstOrDefault();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public void saveChanges()
	{
		_SGOFCTX.SaveChanges();
	}

	public void actualizarVagao(Bi2 vagao)
	{
		Bi2 vagaoExistente = _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.UVagno == vagao.UVagno && bi2.Bostamp == vagao.Bostamp).FirstOrDefault();
		if (vagaoExistente != null)
		{
			vagaoExistente.UEta = vagao.UEta ?? "";
			vagaoExistente.UEtafrt = vagao.UEta ?? "";
			vagaoExistente.UUltdtrep = vagao.UUltdtrep ?? "";
			vagaoExistente.UStatus = vagao.UStatus ?? "";
			vagaoExistente.UVagcod = vagao.UVagcod ?? "";
			vagaoExistente.UVagvol = ((!vagao.UVagvol.HasValue) ? new decimal?(default(decimal)) : vagao.UVagvol);
			vagaoExistente.UPeso = ((!vagao.UPeso.HasValue) ? new decimal?(default(decimal)) : vagao.UPeso);
			vagaoExistente.Bostamp = vagao.Bostamp ?? "";
			vagaoExistente.UComboio = vagao.UComboio ?? "";
			vagaoExistente.UContdcod = vagao.UContdcod ?? "";
			vagaoExistente.UDatcarrg = vagao.UDatcarrg ?? "";
			vagaoExistente.UDsccmcag = vagao.UDsccmcag ?? "";
			vagaoExistente.UAdmintr = vagao.UAdmintr ?? "";
			vagaoExistente.UEncerrno = vagao.UEncerrno ?? "";
			vagaoExistente.UEncrradm = vagao.UEncrradm ?? "";
			vagaoExistente.UUltmtmst = vagao.UUltmtmst ?? "";
			vagaoExistente.UVagdesc = vagao.UVagdesc ?? "";
			vagaoExistente.UChegfrt = ((!vagao.UChegfrt.HasValue) ? new bool?(false) : vagao.UChegfrt);
			vagaoExistente.UVagtip = vagao.UVagtip ?? "";
			vagaoExistente.UDualcon = ((!vagao.UDualcon.HasValue) ? new bool?(false) : vagao.UDualcon);
			vagaoExistente.UTotcont = ((!vagao.UTotcont.HasValue) ? new decimal?(default(decimal)) : vagao.UTotcont);
			vagaoExistente.UStact = vagao.UStact ?? "";
			vagaoExistente.UFullempy = ((!vagao.UDualcon.HasValue) ? new bool?(false) : vagao.UFullempy);
			vagaoExistente.UCodactst = vagao.UCodactst ?? "";
			_SGOFCTX.SaveChanges();
		}
	}

	public VeiculoFerroviario GetVeiculoFerroviarioByNo(string no)
	{
		return _SGOFCTX.VeiculoFerroviario.Where((VeiculoFerroviario vf) => vf.No.Trim() == no).FirstOrDefault();
	}

	public AdmnistracaoVizinha GetAdmnistracaoVizinhaVeiculoVazioByno(string no)
	{
		IQueryable<AdmnistracaoVizinha> query = from veiculoFerroviario in _SGOFCTX.VeiculoFerroviario
			join admnistracaoVizinha in _SGOFCTX.AdmnistracaoVizinha on veiculoFerroviario.Admin equals admnistracaoVizinha.Codigo
			where veiculoFerroviario.No.Trim() == no
			select admnistracaoVizinha;
		return query.FirstOrDefault();
	}

	public void overrideVagoes(Dossier dossier)
	{
		List<Bi2> vagoesToRemove = _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.Bostamp == dossier.bo.Bostamp).ToList();
		_SGOFCTX.Bi2.RemoveRange(vagoesToRemove);
		_SGOFCTX.SaveChanges();
		inserirVagoes(dossier.bi, dossier.bi2, dossier.bo);
	}

	public List<Bi2> getListaVagoesConsignacao(string consignacaostamp)
	{
		return _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.Bostamp == consignacaostamp).ToList();
	}

	public decimal? getMaxLordem(string consignacaostamp)
	{
		return (from bi in _SGOFCTX.Bi
			where bi.Bostamp == consignacaostamp
			select bi.Lordem).ToList().DefaultIfEmpty(default(decimal)).Max() + (decimal?)1;
	}

	public void inserirVagaoSeNaoExiste(Bi2 vagao, string consignacaostamp, Dossier consignacao)
	{
		Bi2 vagaoData = (from bi2 in _SGOFCTX.Bi2.AsNoTracking()
			where bi2.Bostamp == consignacaostamp && bi2.UVagno == vagao.UVagno
			select bi2).FirstOrDefault();
		if (vagaoData == null)
		{
			string vagaostamp = "OFWS" + 21.UseThisSizeForStamp();
			Bi bi3 = new Bi();
			bi3.Bistamp = vagaostamp;
			bi3.Bostamp = consignacaostamp;
			bi3.Ndos = consignacao.bo.Ndos;
			bi3.Nmdos = consignacao.bo.Nmdos;
			bi3.Obrano = consignacao.bo.Obrano;
			bi3.Dataobra = consignacao.bo.Dataobra;
			bi3.No = consignacao.bo.No;
			bi3.Nome = consignacao.bo.Nome;
			vagao.Bi2stamp = vagaostamp;
			vagao.Bostamp = consignacaostamp;
			vagao.UEtaini = vagao.UEta;
			vagao.UEtanot = vagao.UEta;
			vagao.AssignDefaultValues();
			bi3.AssignDefaultValues();
			_SGOFCTX.Entry(vagao).State = EntityState.Added;
			_SGOFCTX.Entry(bi3).State = EntityState.Added;
			List<UBocont> contentores = vagao.contentores;
			if (contentores != null && contentores.Count > 0)
			{
				gravarContentores(contentores, vagaostamp, consignacaostamp);
			}
			_SGOFCTX.SaveChanges();
		}
	}

	public void actualizarTotalVeiculos(decimal? totalVeiculos, string consignacaostamp)
	{
		Bo3 consignacaoExiste = _SGOFCTX.Bo3.Where((Bo3 bo3) => bo3.Bo3stamp == consignacaostamp).FirstOrDefault();
		if (consignacaoExiste != null)
		{
			consignacaoExiste.UTotveicl = totalVeiculos;
		}
		_SGOFCTX.SaveChanges();
	}

	public bool? consignacaoExiste(string consignacao)
	{
		return _SGOFCTX.Bo3.Where((Bo3 bo3) => bo3.UConsgno == consignacao).Any();
	}

	public decimal? getMaxClNo()
	{
		return _SGOFCTX.Cl.Select((Cl cl) => cl.No).ToList().DefaultIfEmpty(default(decimal))
			.Max() + (decimal?)1;
	}

	public Cl getClienteByNuit(string nuit)
	{
		return _SGOFCTX.Cl.Where((Cl cl) => cl.Ncont == nuit).FirstOrDefault();
	}

	public decimal? getMaxAgente()
	{
		return _SGOFCTX.AgenteTransitario.Select((AgenteTransitario agente) => agente.No).ToList().DefaultIfEmpty(default(decimal))
			.Max() + (decimal?)1;
	}

	public AgenteTransitario getAgenteByNuit(string nuit)
	{
		return _SGOFCTX.AgenteTransitario.Where((AgenteTransitario agente) => agente.Nuit == nuit).FirstOrDefault();
	}

	public Bo3 getConsignacaoSemDossier(string consigno)
	{
		return _SGOFCTX.Bo3.Where((Bo3 consignacao) => consignacao.UConsgno == consigno).FirstOrDefault();
	}

	public void testCons()
	{
		IQueryable<Bo> test = _SGOFCTX.Bo.Where((Bo bo) => bo.Ndos == (decimal?)999m);
	}

	public bool? vagaoLargado(string stampVagao, string stampComboio)
	{
		IQueryable<LinhasManobra> query = from manobra in _SGOFCTX.Manobras
			join linhasManobra in _SGOFCTX.LinhasManobra on manobra.ManobraStamp equals linhasManobra.ManobraStamp
			where linhasManobra.Stampevg == stampVagao && manobra.Tipo == "Larga de Vagões"
			select new LinhasManobra
			{
				Novg = linhasManobra.Novg
			};
		return query.Any();
	}

	public List<Dossier> GetConsignacoesPorNotificar(string operacao, string entidade, string sentido, DateTime data)
	{
		IQueryable<Dossier> query = from bo in _SGOFCTX.Bo
			join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			where bo.Dataobra >= data && bo3.UFluxocom.Length > 0 && bo.Ndos == (decimal?)999m && bo3.UConsgno == "8099158782" && !(from un in _SGOFCTX.UNotific
				where un.Entidade == entidade && un.Operacao == operacao && un.Sentido == sentido 
                                                                                                    select un.Tabstamp).Contains(bo3.UConsgno)
			select new Dossier
			{
				bi2 = (from bi2 in _SGOFCTX.Bi2
					where bi2.Bostamp == bo.Bostamp
					select new Bi2
					{
						UVagno = bi2.UVagno,
						UDsccmcag = bi2.UDsccmcag,
						UCntdesc = bi2.UCntdesc,
						UDatcarrg = bi2.UDatcarrg,
						UHorcarrg = bi2.UHorcarrg,
						UPeso = bi2.UPeso,
						UFullempy = bi2.UFullempy
					}).ToList(),
				bo3 = new Bo3
				{
					Bo3stamp = bo3.Bo3stamp,
					UFluxocom = bo3.UFluxocom,
					UDesvcarg = bo3.UDesvcarg,
					UStcarrg = bo3.UStcarrg,
					UConsgno = bo3.UConsgno
				},
				bo = new Bo
				{
					Bostamp = bo.Bostamp
				}
			};
		return query.ToList();
	}

	public UTopup GetUTopupByTipo(string tipo, string numero)
	{
		return _SGOFCTX.UTopup.Where((UTopup topup) => topup.Tipo == tipo && topup.Pedidoid == numero).FirstOrDefault();
	}

	public List<Dossier> GetConsignacoesPorNotificarIncluirOperacao(string operacao, string operacaoToInclude, string entidade, string sentido, DateTime data)
	{
		IQueryable<Dossier> query = from bo in _SGOFCTX.Bo
			join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			where bo.Dataobra >= data && bo3.UFluxocom.Length > 0 && bo.Ndos == (decimal?)999m && !(from un in _SGOFCTX.UNotific
				where un.Entidade == entidade && un.Operacao == operacao && un.Sentido == sentido
				select un.Tabstamp).Contains(bo3.UConsgno) && (from un in _SGOFCTX.UNotific
				where un.Entidade == entidade && un.Operacao == operacaoToInclude && un.Sentido == sentido
				select un.Tabstamp).Contains(bo3.UConsgno)
			select new Dossier
			{
				bi2 = (from bi2 in _SGOFCTX.Bi2
					where bi2.Bostamp == bo.Bostamp
					select new Bi2
					{
						UVagno = bi2.UVagno,
						UDsccmcag = bi2.UDsccmcag,
						UCntdesc = bi2.UCntdesc,
						UDatcarrg = bi2.UDatcarrg,
						UHorcarrg = bi2.UHorcarrg,
						UPeso = bi2.UPeso,
						UFullempy = bi2.UFullempy
					}).ToList(),
				bo3 = new Bo3
				{
					Bo3stamp = bo3.Bo3stamp,
					UFluxocom = bo3.UFluxocom,
					UDesvcarg = bo3.UDesvcarg,
					UStcarrg = bo3.UStcarrg,
					UConsgno = bo3.UConsgno
				},
				bo = new Bo
				{
					Bostamp = bo.Bostamp
				}
			};
		return query.ToList();
	}

	public List<LinhasManobra> GetLinhasManobrasByPedidoId(string pedidoId)
	{
		IQueryable<LinhasManobra> query = from manobras in _SGOFCTX.Manobras
			join linhasManobra in _SGOFCTX.LinhasManobra on manobras.ManobraStamp equals linhasManobra.ManobraStamp
			where manobras.Pedidoid == pedidoId
			select linhasManobra;
		return query.ToList();
	}

	public List<ComboioRegisto> GetListComboiosPorNotificar()
	{
		return _SGOFCTX.ComboioRegisto.Where((ComboioRegisto comboio) => comboio.Notificar == "NOTIFICAR" && comboio.ComboioRegistoStamp== "val25120582164,511915246").ToList();
	}

	public ComboioNotificacao GetComboioNotificacao(string stampComboio)
	{
		IQueryable<LocalizacaoVagao> vagoes = from entidadeVagao in _SGOFCTX.EntidadeVagao
			join linhaVeiculoFerroviario in _SGOFCTX.LinhaVeiculoFerroviario on entidadeVagao.EntidadeVagaostamp equals linhaVeiculoFerroviario.Stampevag
			join comboioRegisto in _SGOFCTX.ComboioRegisto on linhaVeiculoFerroviario.Stampcomboio equals comboioRegisto.ComboioRegistoStamp
			where linhaVeiculoFerroviario.Stampcomboio == stampComboio
			select new LocalizacaoVagao
			{
				wagonNumber = entidadeVagao.No,
				consignmentNumber = ((comboioRegisto.Sentido == "D") ? entidadeVagao.Consgno : entidadeVagao.Consorig),
				wagonStatus = ((entidadeVagao.Estado == "Carregado") ? "loaded" : "empty"),
				weight = entidadeVagao.Qtdton,
				cargo = entidadeVagao.Carga,
				detached = false
			};
		List<Tempos> tempos = _SGOFCTX.Tempos.Where((Tempos tempo) => tempo.ComboioRegistoStamp == stampComboio).ToList();
		ComboioNotificacao comboioNotificacao = new ComboioNotificacao();
		comboioNotificacao.vagoes = vagoes.Where((LocalizacaoVagao vg) => vg.consignmentNumber.Length > 0).ToList();
		comboioNotificacao.tempos = tempos;
		return comboioNotificacao;
	}

	public List<Comboio> GetDadosNotificacaoEstacao(string operacao, string entidade, string sentido)
	{
		var query = (from bo3 in _SGOFCTX.Bo3
			join bo in _SGOFCTX.Bo on bo3.Bo3stamp equals bo.Bostamp
			select new
			{
				Bo3 = bo3,
				Bo = bo
			} into j1
			join EntidadeVagao in _SGOFCTX.EntidadeVagao on j1.Bo3.UConsgno equals EntidadeVagao.Consgno
			select new { j1.Bo3, j1.Bo, EntidadeVagao } into j2
			join LinhaVeiculoFerroviario in _SGOFCTX.LinhaVeiculoFerroviario on j2.EntidadeVagao.EntidadeVagaostamp equals LinhaVeiculoFerroviario.Stampevag
			select new { j2.Bo3, j2.Bo, j2.EntidadeVagao, LinhaVeiculoFerroviario } into j3
			join ComboioRegisto in _SGOFCTX.ComboioRegisto on j3.LinhaVeiculoFerroviario.ComboioRegistoStamp equals ComboioRegisto.ComboioRegistoStamp
			select new { j3.Bo3, j3.Bo, j3.EntidadeVagao, j3.LinhaVeiculoFerroviario, ComboioRegisto } into joined
			where joined.Bo.Ndos == (decimal?)999m && joined.EntidadeVagao.Fechado == (bool?)false && !string.IsNullOrEmpty(joined.EntidadeVagao.Consgno) && joined.ComboioRegisto.Sentido == "D"
			select new { joined.ComboioRegisto, joined.EntidadeVagao }).ToList();
		List<string> uNotificTabstamps = (from uNotific in _SGOFCTX.UNotific
			where uNotific.Operacao == operacao && uNotific.Sentido == sentido && uNotific.Entidade == entidade
			select uNotific.Tabstamp).ToList();
		return (from joined in query
			where !uNotificTabstamps.Contains("")
			group joined by joined.ComboioRegisto.Numero into grouped
			select new Comboio
			{
				dadosComboio = grouped.First().ComboioRegisto,
				veiculos = grouped.Select(j => new LocalizacaoVagao
				{
					wagonNumber = j.EntidadeVagao.No,
					consignmentNumber = j.EntidadeVagao.Consgno,
					wagonStatus = ((j.EntidadeVagao.Estado == "Carregado") ? "loaded" : "empty"),
					weight = j.EntidadeVagao.Qtdton,
					detached = vagaoLargado(j.EntidadeVagao.EntidadeVagaostamp, j.ComboioRegisto.ComboioRegistoStamp)
				}).ToList()
			}).ToList();
	}

	public List<MineDepartureNotificationMPDCDTO> GetConsignacoesPartidaMinaMPDC()
	{
		throw new NotImplementedException();
	}

	public string getVagaoID(string bostamp, string vagno)
	{
		string vagaoID = "";
		Bi2 vagaoIDRes = _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.UVagno == vagno && bi2.Bostamp == bostamp).FirstOrDefault();
		if (vagaoIDRes != null)
		{
			return vagaoIDRes.Bi2stamp;
		}
		return vagaoID;
	}

	public Bo3 GetBo3ByStamp(string bo3stamp)
	{
		return _SGOFCTX.Bo3.Where((Bo3 bo3) => bo3.Bo3stamp == bo3stamp).FirstOrDefault();
	}

	public Bi2 GetVagaoByConsgnoAndVagno(string vagno, string consgno)
	{
		IQueryable<Bi2> query = from bi2 in _SGOFCTX.Bi2
			join bo3 in _SGOFCTX.Bo3 on bi2.Bostamp equals bo3.Bo3stamp
			where bi2.UVagno == vagno && bo3.UConsgno == consgno
			select bi2;
		return query.FirstOrDefault();
	}

	public Bi2 getVagao(string consignacaoID, string vagno)
	{
		return _SGOFCTX.Bi2.Where((Bi2 bi2) => bi2.UVagno == vagno && bi2.Bostamp == consignacaoID).FirstOrDefault();
	}

	public UOridest GetDadosEstacaoByCodigo(string codigo)
	{
		return _SGOFCTX.UOridest.Where((UOridest oridest) => oridest.Codigo == codigo).FirstOrDefault();
	}

	public string getEstacaoByCodigo(string codigo)
	{
		string estacao = "";
		UOridest dadosEstacao = _SGOFCTX.UOridest.Where((UOridest oridest) => oridest.Codigo == codigo).FirstOrDefault();
		if (dadosEstacao != null)
		{
			estacao = dadosEstacao.Cidade;
		}
		return estacao;
	}

	public PlanoManobra GetPlanoMabobraByPedidoID(string pedidoID)
	{
		return (from cabecalho in _SGOFCTX.CabecalhoPlanoManobra
			where cabecalho.PedidoId == pedidoID
			select new PlanoManobra
			{
				cabecalhoPlanoManobra = cabecalho,
				linhasPlanoManobras = _SGOFCTX.LinhasPlanoManobra.Where((LinhasPlanoManobra linha) => linha.CabecalhoPlanoManobraStamp == cabecalho.CabecalhoPlanoManobraStamp).ToList()
			})?.FirstOrDefault();
	}

	public VagaoVeiculoFerroviarioDTO GetEntidadeVagao(string vagno, string consgno)
	{
		return (from entidadeVagao in _SGOFCTX.EntidadeVagao
			where entidadeVagao.No == vagno && entidadeVagao.Consgno == consgno
			select new VagaoVeiculoFerroviarioDTO
			{
				entidadeVagao = entidadeVagao
			})?.FirstOrDefault();
	}

	public int AprovarFornecimentoVagao(string pedidoID)
	{
		return _SGOFCTX.CabecalhoPlanoManobra.Where((CabecalhoPlanoManobra cabecalho) => cabecalho.PedidoId == pedidoID).Update((CabecalhoPlanoManobra cabupdt) => new CabecalhoPlanoManobra
		{
			Aprovado = true,
			Dataaprovacao = DateTime.Now.Date,
			Horaaprovacao = DateTime.Now.ToString("HH:mm")
		});
	}

	public EntidadeVagao GetEntidadeVagaoByStamp(string stamp)
	{
		return _SGOFCTX.EntidadeVagao.Where((EntidadeVagao entidade) => entidade.EntidadeVagaostamp == stamp).FirstOrDefault();
	}

	public PatiosManobra GetPatioManobraByCodigo(string codigo)
	{
		return _SGOFCTX.PatiosManobra.Where((PatiosManobra patio) => patio.Codigo == codigo).FirstOrDefault();
	}

	public LinhaVeiculoFerroviario GetLinhaVeiculoByStampVag(string stampVag)
	{
		return _SGOFCTX.LinhaVeiculoFerroviario.Where((LinhaVeiculoFerroviario linha) => linha.Stampevag == stampVag).FirstOrDefault();
	}

	public DesengateComboio GetDesengateComboioByComboioStamp(string comboioStamp)
	{
		IQueryable<DesengateComboio> query = from desengate in _SGOFCTX.DesengateComboio
			join comboio in _SGOFCTX.ComboioRegisto on desengate.Ref equals comboio.Ref
			where comboio.ComboioRegistoStamp == comboioStamp
			select new DesengateComboio
			{
				Estacao = desengate.Estacao,
				Data = desengate.Data,
				Desiglocal = desengate.Desiglocal,
				Local = desengate.Local,
				Hora = desengate.Hora,
				Ref = desengate.Ref
			};
		return query.FirstOrDefault();
	}

	public PlanoManobra GetPlanoManobraByPedido(string pedidoID)
	{
		return (from cabecalho in _SGOFCTX.CabecalhoPlanoManobra
			where cabecalho.PedidoId == pedidoID
			select new PlanoManobra
			{
				cabecalhoPlanoManobra = cabecalho,
				linhasPlanoManobras = _SGOFCTX.LinhasPlanoManobra.Where((LinhasPlanoManobra linha) => linha.CabecalhoPlanoManobraStamp == cabecalho.CabecalhoPlanoManobraStamp).ToList()
			})?.FirstOrDefault();
	}

	public List<EntidadeVagao> GetLinhasManobraById(string pedidoID)
	{
		IQueryable<EntidadeVagao> query = from manobra in _SGOFCTX.Manobras
			join linhas in _SGOFCTX.LinhasManobra on manobra.ManobraStamp equals linhas.ManobraStamp
			join entidadeVagao in _SGOFCTX.EntidadeVagao on linhas.Stampevg equals entidadeVagao.EntidadeVagaostamp
			where manobra.Pedidoid == pedidoID
			select entidadeVagao;
		return query.ToList();
	}

	public Manobras GetManobraById(string id, string tipo)
	{
		return _SGOFCTX.Manobras.Where((Manobras manobra) => manobra.Pedidoid == id && manobra.Tipo == tipo).FirstOrDefault();
	}

	public List<Manobras> GetManobrasByIdAndTipo(string id, string tipo)
	{
		return _SGOFCTX.Manobras.Where((Manobras manobra) => manobra.Pedidoid == id && manobra.Tipo == tipo).ToList();
	}

	public List<LinhasManobra> GetLinhasManobras(string manobrastamp)
	{
		return _SGOFCTX.LinhasManobra.Where((LinhasManobra manobra) => manobra.ManobraStamp == manobrastamp).ToList();
	}

	public Manobras GetManobra(string id)
	{
		return _SGOFCTX.Manobras.Where((Manobras manobra) => manobra.Pedidoid == id).FirstOrDefault();
	}

	public int AprovarRetiradaVagao(string pedidoID)
	{
		return _SGOFCTX.CabecalhoPedidoRetirada.Where((CabecalhoPedidoRetirada cabecalho) => cabecalho.Pedidoid == pedidoID).Update((CabecalhoPedidoRetirada cabupdt) => new CabecalhoPedidoRetirada
		{
			Aprovado = true,
			Dataaprovacao = DateTime.Now.Date,
			Horaaprovacao = DateTime.Now.ToString("HH:mm")
		});
	}

	public CabecalhoPlanoManobra GetPlanoManobraByPedidoId(string pedidoID)
	{
		return _SGOFCTX.CabecalhoPlanoManobra.Where((CabecalhoPlanoManobra cabecalho) => cabecalho.PedidoId == pedidoID).FirstOrDefault();
	}

	public CabecalhoPlanoManobra GetPlanoMabobraByPedidoIDAndTipo(string pedidoID, string tipo)
	{
		return _SGOFCTX.CabecalhoPlanoManobra.Where((CabecalhoPlanoManobra cabecalho) => cabecalho.PedidoId == pedidoID && cabecalho.Tipo == tipo).FirstOrDefault();
	}

	public CabecalhoPedidoRetirada GetCabecalhoPedidoRetiradaByPedidoId(string pedidoID)
	{
		return _SGOFCTX.CabecalhoPedidoRetirada.Where((CabecalhoPedidoRetirada cabecalho) => cabecalho.Pedidoid == pedidoID).FirstOrDefault();
	}

	public LinhasPlanoManobra GetVagaoFornecimentoByVagnoAndPedido(string consgno, string pedidoID, string vagno)
	{
		IQueryable<LinhasPlanoManobra> query = from pedidoFornecimento in _SGOFCTX.CabecalhoPlanoManobra
			join linhasPedido in _SGOFCTX.LinhasPlanoManobra on pedidoFornecimento.CabecalhoPlanoManobraStamp equals linhasPedido.CabecalhoPlanoManobraStamp
			where pedidoFornecimento.PedidoId == pedidoID && linhasPedido.Novg == vagno && linhasPedido.Consgno == consgno
			select new LinhasPlanoManobra();
		return query.FirstOrDefault();
	}

	public List<EntidadeVagao> GetEntidadeVagaoByPedidoRetiradaStamp(string pedidoRetiradaStamp)
	{
		return _SGOFCTX.EntidadeVagao.Where((EntidadeVagao entidadeVagao) => entidadeVagao.Stamppedret == pedidoRetiradaStamp).ToList();
	}

	public List<EstacaoNotificacaoDestinatario> GetEstacoesNotificacao()
	{
		IQueryable<EstacaoNotificacaoDestinatario> query = from estacaoNot in _SGOFCTX.UEstacnt
			join destnot in _SGOFCTX.UDestnot on estacaoNot.UEstacntstamp equals destnot.UEstacntstamp
			select new EstacaoNotificacaoDestinatario
			{
				estacnt = estacaoNot,
				destnot = destnot
			};
		return query.ToList();
	}

	public int FinalizarManobra(ShuntingDTO shuntingDTO)
	{
		return _SGOFCTX.Manobras.Where((Manobras manobra) => manobra.Pedidoid == shuntingDTO.wagonsRequestId).Update((Manobras up) => new Manobras
		{
			Manobrafinalizada = true,
			Datafim = DateTime.Parse(shuntingDTO.shuntingEndDate).Date,
			Horafim = DateTime.Parse(shuntingDTO.shuntingEndDate).ToString("HH:mm")
		});
	}

	public int IniciarManobra(ShuntingDTO shuntingDTO)
	{
		return _SGOFCTX.Manobras.Where((Manobras manobra) => manobra.Pedidoid == shuntingDTO.wagonsRequestId).Update((Manobras up) => new Manobras
		{
			Manobrainiciada = true,
			Dataini = DateTime.Parse(shuntingDTO.shuntingStartDate).Date,
			Horaini = DateTime.Parse(shuntingDTO.shuntingStartDate).ToString("HH:mm")
		});
	}

	public RotaTrack GetRotaTrack(string origem, string destino)
	{
		IQueryable<RotaTrack> rota = from rt in _SGOFCTX.URota
			where rt.Codest.Trim() == origem && rt.Coddest.Trim() == destino
			select new RotaTrack
			{
				rota = rt,
				trajecto = _SGOFCTX.UTrajrot.Where((UTrajrot trj) => trj.URotastamp == rt.URotastamp).ToList(),
				entidades = _SGOFCTX.UEntrot.Where((UEntrot entrot) => entrot.URotastamp == rt.URotastamp).ToList()
			};
		return rota.FirstOrDefault();
	}

	public string GetUsByInicias(string inicias)
	{
		Us us2 = _SGOFCTX.Us.Where((Us us) => us.Iniciais == inicias.Trim()).FirstOrDefault();
		if (us2 == null)
		{
			return "";
		}
		return us2?.Username;
	}

	public UNotific GetNotificacaoByTabstampAndEntidade(string tabstamp, string entidade)
	{
		return _SGOFCTX.UNotific.Where((UNotific et) => et.Tabstamp == tabstamp && et.Entidade == entidade).FirstOrDefault();
	}

	public UTopup GetTopUpByPedidoAndTipo(string pedido, string tipo)
	{
		return _SGOFCTX.UTopup.Where((UTopup topup) => topup.Pedidoid == pedido && topup.Tipo == tipo).FirstOrDefault();
	}

	public List<UTopuplin> GetLinhasTopUp(string stamp)
	{
		return _SGOFCTX.UTopuplin.Where((UTopuplin topup) => topup.UTopupstamp == stamp).ToList();
	}

	public int ApproveTopUp(string stamp)
	{
		return _SGOFCTX.UTopup.Where((UTopup topup) => topup.UTopupstamp == stamp).Update((UTopup up) => new UTopup
		{
			Aprovado = true
		});
	}
}
