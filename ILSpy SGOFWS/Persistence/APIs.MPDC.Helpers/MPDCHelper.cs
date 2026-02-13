using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Mappers;
using SGOFWS.Persistence.APIs.MPDC.DTOS;
using SGOFWS.Persistence.Contexts;
using SGOFWS.Persistence.DTOS;
using SGOFWS.Services.External;
using SGOFWS.Templates;

namespace SGOFWS.Persistence.APIs.MPDC.Helpers;

public class MPDCHelper : IMPDCHelper
{
	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly RouterMapper routerMapper = new RouterMapper();

	private readonly IConfiguration _configuration;

	private readonly LogHelper logHelper = new LogHelper();

	private readonly SGOFCTX _SGOFCTX;

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	private readonly ILogger<MPDCService> _logger;

	private readonly EmailHelper emailHelper = new EmailHelper();

	private readonly StringTemplates stringTemplates = new StringTemplates();

	private readonly MPDCMapper mpdcMapper = new MPDCMapper();

	private readonly IGenericRepository _genericRepository;

	private readonly MPDCAPI mPDCAPI = new MPDCAPI();

	private readonly APIRouter apiRouter = new APIRouter();

	private readonly GeoHelper geoHelper = new GeoHelper();

	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	public MPDCHelper(IConsignacaoRepository consignacaoRepository, IGenericRepository genericRepository, SGOFCTX SGOFCTX)
	{
		_consignacaoRepository = consignacaoRepository;
		_genericRepository = genericRepository;
		_SGOFCTX = SGOFCTX;
	}

	public List<UpdateStationRequest> MapStations(List<UStqueue> stations)
	{
		List<UpdateStationRequest> mappedStation = new List<UpdateStationRequest>();
		List<UStqueue> orderedStations = stations.OrderBy((UStqueue s) => s.Ord).ToList();
		List<UStqueue> stqueuesToRemove = new List<UStqueue>();
		foreach (UStqueue updateStationRequestBody in orderedStations)
		{
			if (updateStationRequestBody != null && updateStationRequestBody.Operationdate.Year == 1900)
			{
				continue;
			}
			List<UpdateStationWagonDTO> wagons = JsonConvert.DeserializeObject<List<UpdateStationWagonDTO>>(updateStationRequestBody.Wagonsdata);
			MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<UpdateStationWagonDTO, UpdateStationRequestWagon>();
			});
			Mapper mapper = new Mapper(config);
			List<UpdateStationRequestWagon> mappedWagons = mapper.Map<List<UpdateStationRequestWagon>>(wagons);
			List<UpdateStationRequestWagon> updateRequestWagons = new List<UpdateStationRequestWagon>();
			foreach (UpdateStationRequestWagon wagon in mappedWagons)
			{
				updateRequestWagons.Add(wagon);
			}
			if (updateRequestWagons != null && updateRequestWagons.Count() > 0)
			{
				UpdateStationRequest upd = new UpdateStationRequest
				{
					station = updateStationRequestBody.Station,
					stationCode = updateStationRequestBody.Stationcode,
					etaToNextStation = updateStationRequestBody.Etanextst.ToString(),
					operationDate = updateStationRequestBody.Operationdate.ToString(),
					nextStation = updateStationRequestBody?.Nextstation,
					nextStationCode = updateStationRequestBody?.Nextstcode,
					etaToLastStation = updateStationRequestBody?.Etadestination.ToString(),
					trainNumber = updateStationRequestBody.Trainnumber,
					trainReference = updateStationRequestBody.Trainreference,
					trainOrientation = updateStationRequestBody.Trainorientation,
					operationType = updateStationRequestBody?.Operationtype,
					wagons = updateRequestWagons
				};
				mappedStation.Add(upd);
			}
		}
		return mappedStation;
	}
}
