using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Services;

public class OPFService : IOPFService
{
	private readonly IGenericRepository _genericRepository;

	private readonly IOPFRepository _iOPFRepository;

	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly SGOFCTX _SGOFCTX;

	private readonly LogHelper logHelper = new LogHelper();

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	private readonly IConsignmentService _consignmentService;

	public OPFService(IGenericRepository genericRepository, SGOFCTX SGOFCTX, IOPFRepository oPFRepository, IConsignacaoRepository consignacaoRepository, IConsignmentService consignmentService)
	{
		_genericRepository = genericRepository;
		_SGOFCTX = SGOFCTX;
		_iOPFRepository = oPFRepository;
		_consignacaoRepository = consignacaoRepository;
		_consignmentService = consignmentService;
	}

	public string GerarNumeroDaRevisao(DateTime dataini)
	{
		string snR = "";
		int MaxAno = 0;
		decimal myNr = default(decimal);
		DateTime? maxDataIni = _SGOFCTX.RevisaoMaterial.Max((RevisaoMaterial u) => u.Dtirev);
		if (maxDataIni.HasValue)
		{
			MaxAno = maxDataIni.Value.Year;
		}
		if (DateTime.Now.Year == MaxAno && dataini.Year == MaxAno)
		{
			DateTime dateParameter = new DateTime(2024, 3, 1);
			string maxNoQuery = "SELECT MAX(no) AS no FROM u_fer084 WHERE YEAR(u_fer084.dtirev) = YEAR({1})";
			string maxNo = (from x in _SGOFCTX.RevisaoMaterial.FromSqlRaw(maxNoQuery, dataini, dataini).AsNoTracking()
				select x.No).FirstOrDefault();
			long currentYear = DateTime.Now.Year;
			long mnr = 0L;
			try
			{
				mnr = long.Parse(maxNo);
			}
			catch (Exception)
			{
			}
			if (mnr != 0L)
			{
				myNr = mnr - long.Parse((currentYear * 10000000).ToString());
			}
			if (myNr == 0m)
			{
				myNr = 1m;
			}
			else
			{
				myNr += 1m;
			}
			string myNrString = myNr.ToString();
			snR = DateTime.Now.Year + myNrString.PadLeft(7, '0');
		}
		return snR;
	}

	public string GerarNumeroDaComposicao(DateTime dataini)
	{
		string snR = "";
		int MaxAno = 0;
		decimal myNr = default(decimal);
		DateTime? maxDataIni = _SGOFCTX.Composicao.Max((Composicao u) => u.Dataini);
		if (maxDataIni.HasValue)
		{
			MaxAno = maxDataIni.Value.Year;
		}
		if (DateTime.Now.Year == MaxAno && dataini.Year == MaxAno)
		{
			DateTime dateParameter = new DateTime(2024, 3, 1);
			string maxNoQuery = "SELECT MAX(no) AS no FROM u_fer10055 WHERE YEAR(u_fer10055.dataini) = YEAR({1})";
			string maxNo = (from x in _SGOFCTX.Composicao.FromSqlRaw(maxNoQuery, dataini, dataini).AsNoTracking()
				select x.No).FirstOrDefault();
			long currentYear = DateTime.Now.Year;
			long mnr = 0L;
			try
			{
				mnr = long.Parse(maxNo);
			}
			catch (Exception)
			{
			}
			if (mnr != 0L)
			{
				myNr = mnr - long.Parse((currentYear * 10000000).ToString());
			}
			if (myNr == 0m)
			{
				myNr = 1m;
			}
			else
			{
				myNr += 1m;
			}
			string myNrString = myNr.ToString();
			snR = DateTime.Now.Year + myNrString.PadLeft(7, '0');
		}
		return snR;
	}

	public string GetNumeroSequencialRecebimento(DateTime dataini)
	{
		string snR = "";
		int MaxAno = 0;
		decimal myNr = default(decimal);
		DateTime? maxDataIni = _SGOFCTX.Recebimento.Max((Recebimento u) => u.Data);
		if (maxDataIni.HasValue)
		{
			MaxAno = maxDataIni.Value.Year;
		}
		if (DateTime.Now.Year == MaxAno && dataini.Year == MaxAno)
		{
			DateTime dateParameter = new DateTime(2024, 3, 1);
			string maxNoQuery = "SELECT MAX(no) AS no FROM U_FER014 WHERE YEAR(U_FER014.data) = YEAR({1})";
			string maxNo = (from x in _SGOFCTX.Recebimento.FromSqlRaw(maxNoQuery, dateParameter, dateParameter).AsNoTracking()
				select x.No).FirstOrDefault();
			long currentYear = DateTime.Now.Year;
			long mnr = long.Parse(maxNo);
			if (mnr != 0L)
			{
				myNr = mnr - long.Parse((currentYear * 10000000).ToString());
			}
			if (myNr == 0m)
			{
				myNr = 1m;
			}
			else
			{
				myNr += 1m;
			}
			string myNrString = myNr.ToString();
			snR = DateTime.Now.Year + myNrString.PadLeft(7, '0');
		}
		return snR;
	}

	public List<Dossier> GerarConsignacoesPelaComposicao(ComposicaoMapeadaDTO composicaoMapeada)
	{
		List<Composicao> composicoes = composicaoMapeada.composicoes;
		List<LinhaComposicao> linhasComposicao = composicaoMapeada.linhasComposicao;
		ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
		List<LinhaComposicao> distinctLinhas = (from l in linhasComposicao
			where l.Adicionado
			group l by l.Consgno into g
			select g.First()).ToList();
		List<Dossier> dossiers = new List<Dossier>();
		foreach (LinhaComposicao consignacao in distinctLinhas)
		{
			string dossierStamp = 25.UseThisSizeForStamp();
			List<LinhaComposicao> linhas = linhasComposicao.Where((LinhaComposicao l) => l.Consgno == consignacao.Consgno).ToList();
			Composicao dadosComposicao = composicoes.Find((Composicao c) => c.Composicaostamp == linhas.First().Composicaostamp);
			Dossier dossier = new Dossier
			{
				bo = new Bo
				{
					Bostamp = dossierStamp
				},
				bo2 = new Bo2
				{
					Bo2stamp = dossierStamp
				},
				bo3 = new Bo3
				{
					Bo3stamp = dossierStamp,
					UFluxocom = "",
					UFluxotec = "",
					UConsgtip = "",
					UConsgno = consignacao?.Consgno,
					UStrackin = dadosComposicao?.Codest,
					UFccodstd = "",
					UVerif = true,
					UFcstdest = "",
					UCodstcag = "",
					UStcarrg = "",
					UCoddesvg = "",
					UExped = "",
					UConsgtno = "",
					UCodstdet = "",
					UStdest = "",
					UCoddesvt = "",
					UCmpstamp = dadosComposicao.Composicaostamp,
					UTotveicl = linhas.Count()
				}
			};
			decimal ndos = configuracaoDossier.ndos;
			LinhaComposicao linhaFirst = linhas.FirstOrDefault();
			string nmdos = "Consignação";
			string clnome = "";
			int no = 0;
			decimal? obrano = _consignacaoRepository.getMaxObrano(ndos);
			string admin = linhaFirst?.Admin;
			dossier.bo.Ndos = ndos;
			dossier.bo.Obrano = obrano;
			dossier.bo.Nmdos = nmdos;
			dossier.bo.Nome = clnome;
			dossier.bo.No = no;
			dossier.bo3.UAdmin = admin;
			List<Bi> bis = new List<Bi>();
			List<Bi2> bi2s = new List<Bi2>();
			foreach (LinhaComposicao vagao in linhas)
			{
				Bi bi = new Bi
				{
					Bostamp = dossierStamp
				};
				Bi2 bi2 = new Bi2
				{
					UDatcarrg = "",
					UHorcarrg = "",
					UVagno = vagao.Novg,
					UAdmintr = admin,
					UVagtip = vagao.Vagtip,
					UVagdesc = "",
					UVagcod = "",
					UDualcon = false,
					UContdcod = vagao.Carga,
					UCntdesc = vagao?.Carga,
					UDsccmcag = vagao?.Carga,
					UPeso = vagao.Peso,
					UEncerrno = "",
					UEncrradm = vagao.Admin,
					UComboio = "",
					UProxst = "",
					UCodactst = "",
					UStact = "",
					UFullempy = true,
					UUltdtrep = "",
					UStatus = "",
					UEta = new DateTime(1900, 1, 1).ToString(),
					UEtafrt = new DateTime(1900, 1, 1).ToString(),
					UTotcont = default(decimal),
					UUltmtmst = DateTime.Now.ToString(),
					Usrhora = DateTime.Now.ToString("HH:mm"),
					Usrdata = DateTime.Now,
					Ousrdata = DateTime.Now,
					Ousrhora = DateTime.Now.ToString("HH:mm")
				};
				bis.Add(bi);
				bi2s.Add(bi2);
			}
			dossier.bi2 = bi2s;
			dossier.bi = bis;
			Dossier consignacaoGerada = _consignmentService.ProcessarConsignacao(dossier, admin);
			dossiers.Add(consignacaoGerada);
		}
		return dossiers;
	}

	public bool ProcessarComposicoes(ComposicaoMapeadaDTO composicaoMapeada)
	{
		decimal? requestID = logHelper.generateResponseID();
		IDbContextTransaction transaction = _SGOFCTX.Database.BeginTransaction();
		try
		{
			List<Composicao> composicoes = composicaoMapeada.composicoes;
			List<LinhaComposicao> linhasComposicao = composicaoMapeada.linhasComposicao;
			List<UNotascomp> notasComposicao = composicaoMapeada.notasComp;
			List<LinhaComposicao> distinctLinhas = (from l in linhasComposicao
				where l.Adicionado
				group l by l.Consgno into g
				select g.First()).ToList();
			foreach (Composicao composicao in composicoes)
			{
				Composicao composicaoExistente = _iOPFRepository.GetComposicaoByStamp(composicao.Composicaostamp);
				if (composicaoExistente != null)
				{
					composicao.No = composicaoExistente.No;
				}
				else
				{
					composicao.No = GerarNumeroDaRevisao(composicao.Dataini.Value);
				}
				composicao.Horaini = composicao.Dataini.Value.ToString("HH:mm");
				composicao.Horafim = composicao.Datafim.Value.ToString("HH:mm");
				List<KeyValuePair<string, object>> conditionsComposicao = new List<KeyValuePair<string, object>>();
				conditionsComposicao.Add(new KeyValuePair<string, object>("Composicaostamp", composicao.Composicaostamp));
				List<string> keysToExcludeComposicao = new List<string>();
				keysToExcludeComposicao.Add("Composicaostamp");
				List<LinhaComposicao> linhasComposicaoToRemove = _SGOFCTX.LinhaComposicao.Where((LinhaComposicao lc) => lc.Composicaostamp == composicao.Composicaostamp).ToList();
				_SGOFCTX.LinhaComposicao.RemoveRange(linhasComposicaoToRemove);
				_SGOFCTX.SaveChanges();
				_genericRepository.UpsertEntity(composicao, keysToExcludeComposicao, conditionsComposicao, saveChanges: false);
			}
			foreach (LinhaComposicao linhaComposicao in linhasComposicao)
			{
				List<KeyValuePair<string, object>> conditionsLinhaComposicao = new List<KeyValuePair<string, object>>();
				conditionsLinhaComposicao.Add(new KeyValuePair<string, object>("LinhaComposicaostamp", linhaComposicao.LinhaComposicaostamp));
				List<string> keysToExcludeLinhaComposicao = new List<string>();
				keysToExcludeLinhaComposicao.Add("LinhaComposicaostamp");
				List<UNotascomp> notasComposicaoToRemove = _SGOFCTX.UNotascomp.Where((UNotascomp nc) => nc.LinhaComposicaostamp == linhaComposicao.LinhaComposicaostamp).ToList();
				_SGOFCTX.UNotascomp.RemoveRange(notasComposicaoToRemove);
				_SGOFCTX.SaveChanges();
				_genericRepository.UpsertEntity(linhaComposicao, keysToExcludeLinhaComposicao, conditionsLinhaComposicao, saveChanges: false);
			}
			foreach (UNotascomp notaComposicao in notasComposicao)
			{
				List<KeyValuePair<string, object>> conditionsNotaComposicao = new List<KeyValuePair<string, object>>();
				conditionsNotaComposicao.Add(new KeyValuePair<string, object>("UNotascompstamp", notaComposicao.UNotascompstamp));
				List<string> keysToExcludeNotaComposicao = new List<string>();
				keysToExcludeNotaComposicao.Add("UNotascompstamp");
				_genericRepository.UpsertEntity(notaComposicao, keysToExcludeNotaComposicao, conditionsNotaComposicao, saveChanges: false);
			}
			List<Dossier> consignacoesGeradas = GerarConsignacoesPelaComposicao(composicaoMapeada);
			_SGOFCTX.SaveChanges();
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), "Sucess", JsonConvert.SerializeObject(composicaoMapeada));
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "OPFService.ProcessarComposicoes");
			List<LinhaComposicao> vagoesAdicionados = _SGOFCTX.LinhaComposicao.Where((LinhaComposicao lc) => lc.Adicionado == true).ToList();
			transaction.Commit();
			return true;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(composicaoMapeada));
			logHelper.generateLogJB(finalResponse2, requestID.ToString(), "OPFService.ProcessarComposicoes");
			return false;
		}
	}

	public bool ProcessarRevisoes(RevisaoMaterialMapeadaDTO revisaoMaterialMapeada)
	{
		decimal? requestID = logHelper.generateResponseID();
		IDbContextTransaction transaction = _SGOFCTX.Database.BeginTransaction();
		try
		{
			List<RevisaoMaterial> revisoes = revisaoMaterialMapeada.revisoesMaterial;
			List<LinhaRevisao> linhasRevisoes = revisaoMaterialMapeada.linhasRevisao;
			List<UNotasrev> notasRevisao = revisaoMaterialMapeada.notasRevisao;
			List<LinhaRevisao> linhasRevisaoMaterial = new List<LinhaRevisao>();
			foreach (RevisaoMaterial revisao in revisoes)
			{
				RevisaoMaterial revisaoExistente = _iOPFRepository.GetRevisaoByStamp(revisao.RevisaoMaterialstamp);
				if (revisaoExistente != null)
				{
					revisao.No = revisaoExistente.No;
				}
				else
				{
					revisao.No = GerarNumeroDaRevisao(revisao.Dtirev.Value);
				}
				revisao.Hrirev = revisao.Dtirev?.ToString("HH:mm");
				revisao.Hrfrev = revisao.Dtfrev?.ToString("HH:mm");
				revisao.Sync = true;
				List<KeyValuePair<string, object>> conditionsRevisao = new List<KeyValuePair<string, object>>();
				conditionsRevisao.Add(new KeyValuePair<string, object>("RevisaoMaterialstamp", revisao.RevisaoMaterialstamp));
				List<string> keysToExcludeRevisaoMaterial = new List<string>();
				keysToExcludeRevisaoMaterial.Add("RevisaoMaterialstamp");
				List<LinhaRevisao> linhasRevisaoToRemove = _SGOFCTX.LinhaRevisao.Where((LinhaRevisao lrv) => lrv.RevisaoMaterialstamp == revisao.RevisaoMaterialstamp).ToList();
				_SGOFCTX.LinhaRevisao.RemoveRange(linhasRevisaoToRemove);
				_SGOFCTX.SaveChanges();
				_genericRepository.UpsertEntity(revisao, keysToExcludeRevisaoMaterial, conditionsRevisao, saveChanges: false);
			}
			foreach (LinhaRevisao linhaRevisao in linhasRevisoes)
			{
				List<KeyValuePair<string, object>> conditionsLinhaRevisao = new List<KeyValuePair<string, object>>();
				conditionsLinhaRevisao.Add(new KeyValuePair<string, object>("LinhaRevisaostamp", linhaRevisao.LinhaRevisaostamp));
				List<string> keysToExcludeLinhaRevisao = new List<string>();
				keysToExcludeLinhaRevisao.Add("LinhaRevisaostamp");
				List<UNotasrev> notasRevisaoToRemove = _SGOFCTX.UNotasrev.Where((UNotasrev nrv) => nrv.LinhaRevisaostamp == linhaRevisao.LinhaRevisaostamp).ToList();
				_SGOFCTX.UNotasrev.RemoveRange(notasRevisaoToRemove);
				_SGOFCTX.SaveChanges();
				_genericRepository.UpsertEntity(linhaRevisao, keysToExcludeLinhaRevisao, conditionsLinhaRevisao, saveChanges: false);
			}
			foreach (UNotasrev notaRevisao in notasRevisao)
			{
				List<KeyValuePair<string, object>> conditionsNotaRevisao = new List<KeyValuePair<string, object>>();
				conditionsNotaRevisao.Add(new KeyValuePair<string, object>("UNotasrevstamp", notaRevisao.UNotasrevstamp));
				List<string> keysToExcludeNotaRevisao = new List<string>();
				keysToExcludeNotaRevisao.Add("UNotasrevstamp");
				_genericRepository.UpsertEntity(notaRevisao, keysToExcludeNotaRevisao, conditionsNotaRevisao, saveChanges: false);
			}
			_SGOFCTX.SaveChanges();
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), "Sucess", JsonConvert.SerializeObject(revisaoMaterialMapeada));
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "OPFService.ProcessarRevisoes");
			transaction.Commit();
			return true;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(revisaoMaterialMapeada));
			logHelper.generateLogJB(finalResponse2, requestID.ToString(), "OPFService.ProcessarRevisoes");
			return false;
		}
	}
}
