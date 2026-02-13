using Auth.Application.DTOs;
using Auth.Application.Features.Login;
using Auth.Application.Features.Register;
using Auth.Application.Features.CreateRole;
using Auth.Application.Features.AddUserToRole;
using Auth.Application.Features.GetAllRoles;
using Auth.Application.Features.GetUserRoles;
using Shared.Kernel.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Login endpoint - returns JWT token on successful authentication
    /// </summary>
    /// <param name="model">Login credentials</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Authentication response with token</returns>
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new LoginResponseDto
            {
                Token = string.Empty,
                Expiration = null,
                Allowed = false,
                OutputResponse = "INVALID_REQUEST"
            });
        }

        var command = new LoginCommand(model.Username, model.Password);
        var result = await _mediator.Send(command, ct);

        // Return 200 OK even on failure to maintain compatibility with legacy API
        return Ok(result);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="model">Registration details</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Registration result</returns>
    [HttpPost]
    [Route("register")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new RegisterResponseDto
            {
                Success = false,
                Message = "Invalid request",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
            });
        }

        var command = new RegisterCommand(model.Username, model.Email, model.Password);
        var result = await _mediator.Send(command, ct);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    [HttpGet]
    [Route("roles")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> GetRoles(CancellationToken ct = default)
    {
        var query = new GetAllRolesQuery();
        var roles = await _mediator.Send(query, ct);
        return Ok(roles);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="model">Role creation request</param>
    /// <param name="ct">Cancellation token</param>
    [HttpPost]
    [Route("roles")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new RoleResponseDto
            {
                Success = false,
                Message = "Invalid request"
            });
        }

        var command = new CreateRoleCommand(model.RoleName);
        var result = await _mediator.Send(command, ct);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>
    /// Add user to role
    /// </summary>
    /// <param name="model">Add role request</param>
    /// <param name="ct">Cancellation token</param>
    [HttpPost]
    [Route("users/add-role")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> AddUserToRole([FromBody] AddRoleToUserRequestDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new RoleResponseDto
            {
                Success = false,
                Message = "Invalid request"
            });
        }

        var command = new AddUserToRoleCommand(model.Username, model.Role);
        var result = await _mediator.Send(command, ct);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="ct">Cancellation token</param>
    [HttpGet]
    [Route("users/{username}/roles")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> GetUserRoles(string username, CancellationToken ct = default)
    {
        var query = new GetUserRolesQuery(username);
        var roles = await _mediator.Send(query, ct);
        return Ok(roles);
    }
}
