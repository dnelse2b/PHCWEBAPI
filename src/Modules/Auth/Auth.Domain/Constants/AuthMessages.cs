namespace Auth.Domain.Constants;

public static class AuthMessages
{
    // Success messages
    public const string Authenticated = "User authenticated successfully.";
    public const string UserCreated = "User created successfully.";
    public const string RoleCreated = "Role created successfully.";
    public const string UserAddedToRole = "User added to role successfully.";
    
    // Error messages
    public const string BadCredentials = "Invalid username or password.";
    public const string UserAlreadyExists = "User already exists.";
    public const string RoleAlreadyExists = "Role already exists.";
    public const string UserNotFound = "User not found.";
    public const string RoleNotFound = "Role not found.";
    public const string InternalError = "An internal error occurred. Please try again.";
    public const string InvalidToken = "Invalid or expired token.";
    public const string Unauthorized = "You are not authorized to perform this action.";
}
