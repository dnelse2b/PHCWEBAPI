using Auth.Application.DTOs;
using Auth.Application.Features.Login;
using Auth.Application.Features.Register;
using Auth.Application.Features.CreateRole;
using Auth.Application.Features.AddUserToRole;
using Auth.Application.Features.GetAllRoles;
using Auth.Application.Features.GetUserRoles;
using Shared.Kernel.Authorization;
using Shared.Kernel.Responses;
using Shared.Kernel.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediatR;

namespace Auth.Presentation.Controllers;

/// <summary>
/// Authentication controller
/// Provides endpoints for login, registration, and user management
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login-endpoint")] // 🔴 CRITICAL: 3 attempts/min (anti-brute force)
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model, CancellationToken ct = default)
    {
        var correlationId = HttpContext.GetCorrelationId();
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseDTO.Error(
                new ResponseCodeDTO("0002", "Invalid request - Model validation failed", correlationId),
                data: new { errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) }
            ));
        }

        var command = new LoginCommand(model.Username, model.Password);
        var result = await _mediator.Send(command, ct);

        // Return 200 OK even on failure to maintain compatibility with legacy API
        return Ok(ResponseDTO.Success(data: result, correlationId: correlationId));
    }

}
