using Microsoft.AspNetCore.Mvc;
using MediatR;
using Parameters.Application.Commands;
using Parameters.Application.Queries;
using Parameters.Application.DTOs;

namespace Parameters.API.Controllers;

/// <summary>
/// Controller para gerenciar Parâmetros
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ParametersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ParametersController> _logger;

    public ParametersController(IMediator mediator, ILogger<ParametersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obter todos os parâmetros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ParameterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ParameterDto>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAllParametersQuery(includeInactive);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all parameters");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Obter parâmetro por Stamp
    /// </summary>
    [HttpGet("{e1Stamp}")]
    [ProducesResponseType(typeof(ParameterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParameterDto>> GetByStamp(
        string e1Stamp,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetParameterByStampQuery(e1Stamp);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound($"Parameter with stamp {e1Stamp} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting parameter by stamp {E1Stamp}", e1Stamp);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Criar novo parâmetro
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ParameterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParameterDto>> Create(
        [FromBody] CreateParameterDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateParameterCommand(
                dto.Code,
                dto.Description,
                dto.E4Details,
                User.Identity?.Name
            );

            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetByStamp),
                new { e1Stamp = result.E1Stamp },
                result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when creating parameter");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating parameter");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Atualizar parâmetro
    /// </summary>
    [HttpPut("{e1Stamp}")]
    [ProducesResponseType(typeof(ParameterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParameterDto>> Update(
        string e1Stamp,
        [FromBody] UpdateParameterDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateParameterCommand(
                e1Stamp,
                dto.Code,
                dto.Description,
                dto.Active,
                dto.E4Details,
                User.Identity?.Name
            );

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Parameter not found");
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating parameter");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parameter");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Deletar parâmetro
    /// </summary>
    [HttpDelete("{e1Stamp}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        string e1Stamp,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new DeleteParameterCommand(e1Stamp);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound($"Parameter with stamp {e1Stamp} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting parameter");
            return StatusCode(500, "Internal server error");
        }
    }
}
