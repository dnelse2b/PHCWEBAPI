using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGOFWS.Domains.Interfaces;
using SGOFWS.DTOs;
using SGOFWS.Mappers;
using SGOFWS.Persistence.APIs;

namespace SGOFWS.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
	private readonly ITFRService _TFRService;

	private readonly IMPDCService _MPDCService;

	private readonly IConsignmentService _Consignmentservice;

	private readonly ITrackService _trackService;

	private readonly RouterMapper routerTFRMapper = new RouterMapper();

	private readonly APIRouter apiRouter = new APIRouter();

	public TrackController(ITFRService TFRService, IMPDCService mPDCService, IConsignmentService consignmentservice, ITrackService trackService)
	{
		_TFRService = TFRService;
		_MPDCService = mPDCService;
		_Consignmentservice = consignmentservice;
		_trackService = trackService;
	}

	[HttpGet]
	[Route("Stations")]
	public async Task<ActionResult<ResponseDTO>> GetStationsByRange(DateTime startDate, DateTime endDate)
	{
		ResponseDTO response = _Consignmentservice.GetStationsByRange(startDate, endDate, "MPDC");
		return Ok(response);
	}

	[HttpPost]
	[Route("UpdateTrainStation")]
	public async Task<ActionResult<ResponseDTO>> UpdateTrainStation(TrainUpdateStationDTO trainUpdateStation)
	{
		ResponseDTO response = _trackService.UpdateTrainStation(trainUpdateStation);
		return Ok(response);
	}
}
