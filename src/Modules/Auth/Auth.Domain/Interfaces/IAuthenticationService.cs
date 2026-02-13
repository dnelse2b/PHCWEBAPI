namespace Auth.Domain.Interfaces;

/// <summary>
/// High-level authentication service interface.
/// This abstraction allows for different authentication providers (Identity, Auth0, Firebase, etc.)
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user and returns authentication result
    /// </summary>
    Task<AuthenticationResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<AuthenticationResult> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates if a token is valid
    /// </summary>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Revokes/invalidates a token
    /// </summary>
    Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets user roles
    /// </summary>
    Task<IEnumerable<string>> GetUserRolesAsync(string username, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication result contract
/// </summary>
public class AuthenticationResult
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime? Expiration { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
