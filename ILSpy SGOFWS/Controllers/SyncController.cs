using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SGOFWS.Domains.Interfaces;
using SGOFWS.DTOs;

namespace SGOFWS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SyncController : ControllerBase
{
	private readonly ISyncService _syncService;

	public SyncController(ISyncService syncService)
	{
		_syncService = syncService;
	}

	[HttpPost]
	[Route("SyncData")]
	public async Task<ActionResult<ResponseDTO>> SyncData(SyncDTO syncDTO)
	{
		ResponseDTO response = _syncService.SyncData(syncDTO);
		return Ok(response);
	}
}
