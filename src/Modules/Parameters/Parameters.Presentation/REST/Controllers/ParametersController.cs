using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using Parameters.Application.Features.CreateParameter;
using Parameters.Application.Features.UpdateParameter;
using Parameters.Application.Features.DeleteParameter;
using Parameters.Application.Features.GetAllParameters;
using Parameters.Application.Features.GetParameterByStamp;
using Shared.Kernel.Responses;

namespace Parameters.Presentation.REST.Controllers;

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

    [HttpGet("{paraStamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> GetByStamp(
        string paraStamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetParameterByStampQuery(paraStamp), ct);

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
        var command = new CreateParameterCommand(dto, User.Identity?.Name);
        var result = await _mediator.Send(command, ct);

        return CreatedAtAction(
            nameof(GetByStamp),
            new { paraStamp = result.ParaStamp },
            ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.CreatedSuccessfully));
    }

    [HttpPut("{paraStamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Update(
        string paraStamp,
        [FromBody] UpdateParameterDto dto,
        CancellationToken ct = default)
    {
        var command = new UpdateParameterCommand(paraStamp, dto, User.Identity?.Name);
        var result = await _mediator.Send(command, ct);
        return Ok(ResponseDTO.Success(data: result, content: ResponseCodes.Parameter.UpdatedSuccessfully));
    }

    [HttpDelete("{paraStamp}")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResponseDTO>> Delete(
        string paraStamp,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteParameterCommand(paraStamp), ct);

        return result
            ? Ok(ResponseDTO.Success(content: ResponseCodes.Parameter.DeletedSuccessfully))
            : NotFound(ResponseDTO.Error(ResponseCodes.Parameter.NotFound));
    }
}
