using System.Security.Claims;
using Auth.Domain.Interfaces;
using Auth.Domain.Constants;
using Shared.Kernel.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Services;

/// <summary>
/// Authentication service implementation using ASP.NET Identity
/// This is the concrete implementation that can be replaced with other providers (Auth0, Firebase, etc.)
/// </summary>
public class IdentityAuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<IdentityAuthenticationService> _logger;

    public IdentityAuthenticationService(
        UserManager<IdentityUser> userManager,
        ITokenService tokenService,
        IUserRepository userRepository,
        ILogger<IdentityAuthenticationService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<AuthenticationResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting authentication for user: {Username}", username);
            
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return new AuthenticationResult
                {
                    Success = false,
                    Message = AuthMessages.BadCredentials
                };
            }

            _logger.LogInformation("User found: {Username}, checking password...", username);
            
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password for user: {Username}", username);
                return new AuthenticationResult
                {
                    Success = false,
                    Message = AuthMessages.BadCredentials
                };
            }

            _logger.LogInformation("Password validated for user: {Username}, retrieving roles...", username);
            
            var userRoles = await _userManager.GetRolesAsync(user);
            
            _logger.LogInformation("User {Username} has {RoleCount} roles", username, userRoles.Count);
            
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            _logger.LogInformation("Generating token for user: {Username}", username);
            
            var token = _tokenService.GenerateToken(authClaims);
            var expiration = _tokenService.GetTokenExpiration();

            _logger.LogInformation("Token generated successfully for user: {Username}", username);

            return new AuthenticationResult
            {
                Success = true,
                Token = token,
                Expiration = expiration,
                Message = AuthMessages.Authenticated,
                Roles = userRoles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Exception in LoginAsync for user {Username}. Type: {ExceptionType}, Message: {Message}", 
                username, 
                ex.GetType().FullName,
                ex.Message);
            throw;
        }
    }

    public async Task<AuthenticationResult> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByNameAsync(username);
        if (existingUser != null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = AuthMessages.UserAlreadyExists
            };
        }

        var (success, userId, errors) = await _userRepository.CreateUserAsync(username, email, password, cancellationToken);

        return new AuthenticationResult
        {
            Success = success,
            Message = success ? AuthMessages.UserCreated : string.Join(", ", errors)
        };
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var principal = await _tokenService.ValidateTokenAsync(token);
        return principal != null;
    }

    public Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        // Token revocation would require a token blacklist/cache
        // For now, tokens are stateless and expire naturally
        // This can be enhanced with Redis or database-backed token blacklist
        return Task.FromResult(true);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetUserRolesAsync(username, cancellationToken);
    }
}
