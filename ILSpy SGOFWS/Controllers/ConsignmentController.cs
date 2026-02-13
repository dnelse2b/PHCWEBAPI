using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGOFWS.Domains.Interfaces;
using SGOFWS.DTOs;
using SGOFWS.Jobs;
using SGOFWS.Mappers;
using SGOFWS.Persistence.APIs;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ConsignmentController : ControllerBase
{
	private readonly DossierTFRMapper dossierTFRMapper = new DossierTFRMapper();

	private readonly ConsignacaoJob consignacaoJob = new ConsignacaoJob();

	private readonly ITFRService _TFRService;

	private readonly IMPDCService _MPDCService;

	private readonly IConsignmentService _Consignmentservice;

	private readonly RouterMapper routerTFRMapper = new RouterMapper();

	private readonly APIRouter apiRouter = new APIRouter();

	public ConsignmentController(ITFRService TFRService, IMPDCService mPDCService, IConsignmentService consignmentservice)
	{
		_TFRService = TFRService;
		_MPDCService = mPDCService;
		_Consignmentservice = consignmentservice;
	}

	[HttpPost]
	[Route("UpdateConsignment")]
	public async Task<ActionResult<ResponseDTO>> UpdateConsignment(UpdateConsignmentDataMPDCDTO updateConsignment)
	{
		ResponseDTO response = _MPDCService.UpdateConsignment(updateConsignment);
		return Ok(response);
	}

	[HttpPost]
	[Route("SubmitShunting")]
	public async Task<ActionResult<ResponseDTO>> SubmitShunting(ShuntingDTO shuntingDTO)
	{
		return Ok(await _MPDCService.SubmitShunting(shuntingDTO));
	}

	[HttpPost]
	[Route("TopUpRequest")]
	public async Task<ActionResult<ResponseDTO>> TopUpRequest(TopUpDTO topUp)
	{
		return Ok(await _MPDCService.TopUpRequest(topUp));
	}

	[HttpGet]
	[Route("Shuntings")]
	public async Task<ActionResult<ResponseDTO>> GetManobrasByRange(DateTime startDate, DateTime endDate)
	{
		ResponseDTO response = _Consignmentservice.GetManobrasByRange(startDate, endDate);
		return Ok(response);
	}

	[HttpGet]
	[Route("Request")]
	public async Task<ActionResult<ResponseDTO>> GetRequestById(string requestType, string requestID)
	{
		ResponseDTO response = _Consignmentservice.GetRequestByID(requestType, requestID);
		return Ok(response);
	}

	[HttpPost]
	[Route("requestForWagonSupply")]
	public async Task<ActionResult<ResponseDTO>> RequestForWagonSupply(WagonSupplyRequestDTO wagonSupplyRequestDTO)
	{
		ResponseDTO response = _MPDCService.RequestForWagonSupply(wagonSupplyRequestDTO);
		return Ok(response);
	}

	[HttpPost]
	[Route("requestForWagonWithdrawal")]
	public async Task<ActionResult<ResponseDTO>> RequestForWagonWithdrawal(WagonWithdrawalResquestDTO wagonWithdrawalRequestDTO)
	{
		ResponseDTO response = _MPDCService.RequestForWagonWithdrawal(wagonWithdrawalRequestDTO);
		return Ok(response);
	}

	[HttpPost]
	[Route("approveWagonOperation")]
	public async Task<ActionResult<ResponseDTO>> ApproveWagonOperation(ApproveWagonOperationDTO ApproveWagonOperationDTO)
	{
		return Ok(await _MPDCService.ApproveWagonOperation(ApproveWagonOperationDTO));
	}

	[HttpPost]
	[Route("wagonOffload")]
	public async Task<ActionResult<ResponseDTO>> WagonOffload(WagonOffloadDTO wagonOffloadDTO)
	{
		ResponseDTO response = _MPDCService.WagonOffload(wagonOffloadDTO);
		return Ok(response);
	}

	[HttpPost]
	[Route("closeShunting")]
	public async Task<ActionResult<ResponseDTO>> CloseShuntingAsync(ShuntingDTO shuntingDTO)
	{
		return Ok(await _MPDCService.CloseShuntingAsync(shuntingDTO));
	}

	[HttpPost]
	[Route("startShunting")]
	public async Task<ActionResult<ResponseDTO>> StartShuntingAsync(ShuntingDTO shuntingDTO)
	{
		return Ok(await _MPDCService.StartShuntingAsync(shuntingDTO));
	}

	[HttpGet]
	[Route("consignment")]
	public async Task<ActionResult<ResponseDTO>> GetConsignmentByNumber(string consignmentNumber)
	{
		ResponseDTO response = _Consignmentservice.GetConsignmentByNumber(consignmentNumber);
		return Ok(response);
	}
}
