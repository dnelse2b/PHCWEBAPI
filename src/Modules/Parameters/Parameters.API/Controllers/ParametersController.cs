using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using Parameters.Application.Features.CreateParameter;
using Parameters.Application.Features.UpdateParameter;
using Parameters.Application.Features.DeleteParameter;
using Parameters.Application.Features.GetAllParameters;
using Parameters.Application.Features.GetParameterByStamp;
using Shared.Kernel.Responses;

namespace Parameters.API.Controllers;

[ApiController]
[Route("api/parameters")]
[Produces("application/json")]
public sealed class ParametersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParametersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAllParametersQuery(includeInactive), ct);
        return Ok(ResponseDTO.Success(data: result));
    }

    [HttpGet("{e1Stamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> GetByStamp(
        string e1Stamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetParameterByStampQuery(e1Stamp), ct);

        return result is null
            ? NotFound(ResponseDTO.Error(ResponseCodes.Parameter.NotFound))
            : Ok(ResponseDTO.Success(data: result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Create(
        [FromBody] CreateParameterDto dto,
        CancellationToken ct = default)
    {
        var command = new CreateParameterCommand(
            dto.Code,
            dto.Description,
            dto.E4Details,
            User.Identity?.Name);

        var result = await _mediator.Send(command, ct);

        return CreatedAtAction(
            nameof(GetByStamp),
            new { e1Stamp = result.E1Stamp },
            ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.CreatedSuccessfully));
    }

    [HttpPut("{e1Stamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Update(
        string e1Stamp,
        [FromBody] UpdateParameterDto dto,
        CancellationToken ct = default)
    {
        var command = new UpdateParameterCommand(
            e1Stamp,
            dto.Code,
            dto.Description,
            dto.Active,
            dto.E4Details,
            User.Identity?.Name);

        var result = await _mediator.Send(command, ct);
        return Ok(ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.UpdatedSuccessfully));
    }

    [HttpDelete("{e1Stamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Delete(
        string e1Stamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteParameterCommand(e1Stamp), ct);

        return result
            ? Ok(ResponseDTO.Success(content: ResponseCodes.Parameter.DeletedSuccessfully))
            : NotFound(ResponseDTO.Error(ResponseCodes.Parameter.NotFound));
    }
}

