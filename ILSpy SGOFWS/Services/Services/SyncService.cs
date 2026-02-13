using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Services;

public class SyncService : ISyncService
{
	private readonly LogHelper logHelper = new LogHelper();

	private readonly IGenericRepository _genericRepository;

	private readonly ISyncRepository _syncRepository;

	private readonly IOPFService _IOPFService;

	private readonly SGOFCTX _SGOFCTX;

	private readonly ObjectHelper objectHelper = new ObjectHelper();

	public SyncService(IGenericRepository genericRepository, ISyncRepository syncRepository, SGOFCTX SGOFCTX, IOPFService IOPFService)
	{
		_genericRepository = genericRepository;
		_syncRepository = syncRepository;
		_SGOFCTX = SGOFCTX;
		_IOPFService = IOPFService;
	}

	public SyncService()
	{
	}

	public ResponseDTO SyncData(SyncDTO syncDTO)
	{
		object data = JsonConvert.DeserializeObject<object>(syncDTO?.data?.ToString());
		USync sync = new USync
		{
			USyncstamp = 25.UseThisSizeForStamp(),
			Data = data.ToString(),
			Processcode = syncDTO?.processCode,
			Processid = 25.UseThisSizeForStamp(),
			Ousrdata = DateTime.Now.Date,
			Usrdata = DateTime.Now.Date,
			Ousrhora = DateTime.Now.ToString("HH:mm"),
			Usrhora = DateTime.Now.ToString("HH:mm")
		};
		_genericRepository.Add(sync);
		_genericRepository.SaveChanges();
		decimal? responseID = logHelper.generateResponseID();
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
	}

	public TEntity BuildEntityByObject<TEntity>(Dictionary<string, object> inputObject, SGOFCTX dbContext, string tableName) where TEntity : class, new()
	{
		TEntity entity = new TEntity();
		foreach (KeyValuePair<string, object> kvp in inputObject)
		{
			string columnName = dbContext.GetPropertyNameForColumn(tableName, kvp.Key.ToLower());
			PropertyInfo property = typeof(TEntity).GetProperty(columnName);
			if (!(property != null))
			{
				continue;
			}
			Type propertyType = property.PropertyType;
			object valueToSet = kvp.Value;
			if (valueToSet != null && !propertyType.IsAssignableFrom(valueToSet.GetType()))
			{
				if (propertyType == typeof(DateTime?) && valueToSet.GetType() == typeof(string))
				{
					string dateString = (string)valueToSet;
					valueToSet = (string.IsNullOrWhiteSpace(dateString) ? ((object)new DateTime(1900, 1, 1)) : ((!DateTime.TryParse(dateString, out var dateTimeValue)) ? ((object)new DateTime(1900, 1, 1)) : ((object)dateTimeValue)));
				}
				else if (propertyType == typeof(bool?) && valueToSet.GetType() == typeof(long))
				{
					valueToSet = (long)valueToSet != 0;
				}
				else if (propertyType == typeof(int) && valueToSet.GetType() == typeof(long))
				{
					valueToSet = (int)(long)valueToSet;
				}
				else if (propertyType == typeof(decimal) && valueToSet.GetType() == typeof(double))
				{
					valueToSet = (decimal)(double)valueToSet;
				}
				else if (propertyType == typeof(DateTime) && valueToSet.GetType() == typeof(string))
				{
					string dateString2 = (string)valueToSet;
					valueToSet = (string.IsNullOrWhiteSpace(dateString2) ? ((object)new DateTime(1900, 1, 1)) : ((!DateTime.TryParse(dateString2, out var dateTimeValue2)) ? ((object)new DateTime(1900, 1, 1)) : ((object)dateTimeValue2)));
				}
				else
				{
					try
					{
						valueToSet = Convert.ChangeType(valueToSet, propertyType);
					}
					catch
					{
						if (!(propertyType == typeof(DateTime)) && !(propertyType == typeof(DateTime?)))
						{
							throw;
						}
						valueToSet = new DateTime(1900, 1, 1);
					}
				}
			}
			property.SetValue(entity, valueToSet);
		}
		return entity;
	}

	public bool SincronizarComposicao(USync sync)
	{
		SyncComposicaoDTO composicaoObject = JsonConvert.DeserializeObject<SyncComposicaoDTO>(sync.Data);
		List<Dictionary<string, object>> composicoesObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(composicaoObject.u_fer10055?.ToString());
		List<Dictionary<string, object>> LinhaComposicaoObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(composicaoObject.u_fer1006?.ToString());
		List<Dictionary<string, object>> notasComposicaoObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(composicaoObject.u_notascomp?.ToString());
		List<Composicao> composicoes = new List<Composicao>();
		List<LinhaComposicao> linhasComposicao = new List<LinhaComposicao>();
		List<UNotascomp> notasComposicao = new List<UNotascomp>();
		foreach (Dictionary<string, object> composicaoObj in composicoesObj)
		{
			Composicao composicao = BuildEntityByObject<Composicao>(composicaoObj, _SGOFCTX, "u_fer10055");
			composicoes.Add(composicao);
		}
		foreach (Dictionary<string, object> linhaComposicaoObj in LinhaComposicaoObj)
		{
			LinhaComposicao linhaComposicao = BuildEntityByObject<LinhaComposicao>(linhaComposicaoObj, _SGOFCTX, "u_fer1006");
			linhasComposicao.Add(linhaComposicao);
		}
		foreach (Dictionary<string, object> notaComposicaoObj in notasComposicaoObj)
		{
			UNotascomp notaComposicao = BuildEntityByObject<UNotascomp>(notaComposicaoObj, _SGOFCTX, "u_notascomp");
			notasComposicao.Add(notaComposicao);
		}
		ComposicaoMapeadaDTO composicaoMapeada = new ComposicaoMapeadaDTO
		{
			linhasComposicao = linhasComposicao,
			composicoes = composicoes,
			notasComp = notasComposicao
		};
		return _IOPFService.ProcessarComposicoes(composicaoMapeada);
	}

	public bool SincronizarRevisaoMaterial(USync sync)
	{
		SyncRevisaoMaterialDTO revisaoMaterialObject = JsonConvert.DeserializeObject<SyncRevisaoMaterialDTO>(sync.Data);
		List<Dictionary<string, object>> revisoesObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(revisaoMaterialObject.u_fer084?.ToString());
		List<Dictionary<string, object>> linhaRevisoesObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(revisaoMaterialObject.u_fer085?.ToString());
		List<Dictionary<string, object>> notasRevisoesObj = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(revisaoMaterialObject.u_notasrev?.ToString());
		List<RevisaoMaterial> revisoes = new List<RevisaoMaterial>();
		List<Bi2> bis2 = new List<Bi2>();
		List<LinhaRevisao> linhasRevisao = new List<LinhaRevisao>();
		List<UNotasrev> notasRevisao = new List<UNotasrev>();
		foreach (Dictionary<string, object> revisaoObj in revisoesObj)
		{
			RevisaoMaterial revisao = BuildEntityByObject<RevisaoMaterial>(revisaoObj, _SGOFCTX, "u_fer084");
			revisoes.Add(revisao);
		}
		foreach (Dictionary<string, object> linhaRevisaoObj in linhaRevisoesObj)
		{
			LinhaRevisao linhaRevisao = BuildEntityByObject<LinhaRevisao>(linhaRevisaoObj, _SGOFCTX, "u_fer085");
			linhasRevisao.Add(linhaRevisao);
		}
		foreach (Dictionary<string, object> notaRevisaoObj in notasRevisoesObj)
		{
			UNotasrev notaRevisao = BuildEntityByObject<UNotasrev>(notaRevisaoObj, _SGOFCTX, "u_notasrev");
			notasRevisao.Add(notaRevisao);
		}
		RevisaoMaterialMapeadaDTO revisaoMaterialMapeada = new RevisaoMaterialMapeadaDTO
		{
			linhasRevisao = linhasRevisao,
			revisoesMaterial = revisoes,
			notasRevisao = notasRevisao
		};
		return _IOPFService.ProcessarRevisoes(revisaoMaterialMapeada);
	}

	public void HandleSync()
	{
		List<USync> syncs = _syncRepository.GetSync();
		foreach (USync sync in syncs)
		{
			DateTime currentDate = DateTime.Now;
			DateTime processedDate = sync.Dataproc;
			double minutesDifference = (currentDate - processedDate).TotalMinutes;
			if ((!(sync.Status?.Trim() == "PROCESSING") || !(minutesDifference > 3.0)) && !(sync.Status?.Trim() != "PROCESSING"))
			{
				continue;
			}
			Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(sync.Data);
			sync.Dataproc = DateTime.Now;
			sync.Status = "PROCESSING";
			_genericRepository.SaveChanges();
			string text = sync?.Processcode;
			if (!(text == "REVISAOMATERIAL"))
			{
				if (text == "COMPOSICAO" && SincronizarComposicao(sync))
				{
					_genericRepository.Remove(sync);
					_genericRepository.SaveChanges();
				}
			}
			else if (SincronizarRevisaoMaterial(sync))
			{
				_genericRepository.Remove(sync);
				_genericRepository.SaveChanges();
			}
		}
	}
}
