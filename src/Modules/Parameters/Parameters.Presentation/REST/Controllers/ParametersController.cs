using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Parameters.Application.Features.CreateParameter;
using Parameters.Application.Features.UpdateParameter;
using Parameters.Application.Features.DeleteParameter;
using Parameters.Application.Features.GetAllParameters;
using Parameters.Application.Features.GetParameterByStamp;
using Shared.Kernel.Responses;
using Shared.Kernel.Extensions;
using Shared.Kernel.Authorization;
using Parameters.Application.DTOs.Parameters;

namespace Parameters.Presentation.REST.Controllers;

/// <summary>
/// Parameters controller - Manages business parameters
/// 🛡️ SECURITY: Each endpoint has specific authorization requirements
/// </summary>
[ApiController]
[Route("api/parameters")]
[Produces("application/json")]
public sealed class ParametersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParametersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Get all parameters with optional inactive filter
    /// 🟢 Rate limited to 50 queries per minute
    /// 🔒 OPÇÃO 1 - OR lógico: Requer Rail2Port OU Administrator (qualquer uma)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Rail2Port},{AppRoles.Administrator}")]  // ✅ Usuário precisa ter Rail2Port OU Administrator
    [EnableRateLimiting("parameters-query")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAllParametersQuery(includeInactive), ct);
        var correlationId = HttpContext.GetCorrelationId();
        return Ok(ResponseDTO.Success(data: result, correlationId: correlationId));
    }

    /// <summary>
    /// Get parameter by stamp
    /// 🟢 Rate limited to 50 queries per minute
    /// 🔒 OPÇÃO 2 - AND lógico: Requer Rail2Port E Administrator (ambas obrigatórias)
    /// </summary>
    [HttpGet("{para1Stamp}")]
    [Authorize(Roles = $"{AppRoles.Rail2Port},{AppRoles.Administrator}")]  // ✅ Precisa ter Rail2Port E Administrator
    [EnableRateLimiting("parameters-query")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> GetByStamp(
        string para1Stamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetParameterByStampQuery(para1Stamp), ct);
        var correlationId = HttpContext.GetCorrelationId();

        return result is null
            ? NotFound(ResponseDTO.Error(ResponseCodes.Parameter.NotFound, correlationId: correlationId))
            : Ok(ResponseDTO.Success(data: result, correlationId: correlationId));
    }

    /// <summary>
    /// Create a new parameter
    /// 🟠 Rate limited to 5 creates per minute to prevent spam
    /// 🔒 OPÇÃO 3 - Policy + Role: Requer (ApiAccess) E Rail2Port
    /// ApiAccess = Administrator OU ApiUser OU InternalUser
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AppPolicies.ApiAccess)]   // ✅ Precisa ter Administrator OU ApiUser OU InternalUser
    [Authorize(Roles = AppRoles.Rail2Port)]       // ✅ E TAMBÉM precisa ter Rail2Port
    [EnableRateLimiting("parameters-create")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Create(
        [FromBody] CreateParameterInputDTO dto,
        CancellationToken ct = default)
    {
        var command = new CreateParameterCommand(dto, User.Identity?.Name);
        var result = await _mediator.Send(command, ct);
        var correlationId = HttpContext.GetCorrelationId();

        return CreatedAtAction(
            nameof(GetByStamp),
            new { para1Stamp = result.Para1Stamp },
            ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.CreatedSuccessfully, correlationId: correlationId));
    }

    /// <summary>
    /// Update an existing parameter
    /// 🟡 Rate limited to 10 updates per minute
    /// 🔒 Múltiplas roles com OR: Rail2Port OU ApiUser OU Administrator
    /// </summary>
    [HttpPut("{para1Stamp}")]
    [Authorize(Roles = $"{AppRoles.Rail2Port},{AppRoles.ApiUser},{AppRoles.Administrator}")]  // ✅ Qualquer uma dessas 3 roles
    [EnableRateLimiting("parameters-update")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Update(
        string para1Stamp,
        [FromBody] UpdateParameterInputDTO dto,
        CancellationToken ct = default)
    {
        var command = new UpdateParameterCommand(para1Stamp, dto, User.Identity?.Name);
        var result = await _mediator.Send(command, ct);
        var correlationId = HttpContext.GetCorrelationId();
        return Ok(ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.UpdatedSuccessfully, correlationId: correlationId));
    }

    /// <summary>
    /// Delete a parameter (soft delete)
    /// 🔴 Rate limited to 3 deletes per minute (critical operation)
    /// 🔒 Apenas Administrator (mais restritivo)
    /// </summary>
    [HttpDelete("{para1Stamp}")]
    [Authorize(Roles = AppRoles.Administrator)]  // ✅ Apenas Administrator
    [EnableRateLimiting("parameters-delete")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Delete(
        string para1Stamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteParameterCommand(para1Stamp), ct);
        var correlationId = HttpContext.GetCorrelationId();

        return result
            ? Ok(ResponseDTO.Success(content: ResponseCodes.Parameter.DeletedSuccessfully, correlationId: correlationId))
            : NotFound(ResponseDTO.Error(ResponseCodes.Parameter.NotFound, correlationId: correlationId));
    }
}
