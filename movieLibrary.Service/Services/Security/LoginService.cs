using System.Collections.Concurrent;
using movieLibraryService.Models.DTOs.UserDTOs;
using movieLibraryService.Models.Response;

namespace movieLibraryService.Services.Security;

public record JwtOptions(
    string SecretKey,
    string Issuer,
    string Audience,
    int ExpirationInMinutes
);

public class LoginService
{
    private readonly HashTokens _hashTokens = new();
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    private readonly ConcurrentDictionary<string, UserCredentials> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, RecoveryTicket> _recoveryTickets = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes the LoginService with a default admin user. 
    /// The default credentials are "admin" for the username and "password" for the password.
    /// </summary>
    public LoginService(JwtTokenService jwtTokenService, JwtOptions jwtOptions)
    {
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions;

        SetupOrUpdatePasswordAsync("admin", "password").GetAwaiter().GetResult();
    }

    // --------------------
    // Controller facing Methods
    // --------------------

    /// <summary>
    /// Registers a new user with the provided username and password. 
    /// The method checks if the username and password are valid and if the username already exists. 
    /// If the registration is successful, it returns an ApiResponse with a success message; otherwise, 
    /// it returns an ApiResponse with an appropriate error message and status code.
    /// </summary>
    /// <param name="userRegisterDTO">The DTO containing the user's registration information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An ApiResponse indicating the result of the registration attempt.</returns>
    public async Task<ApiResponse<string>> RegisterUserAsync(UserRegisterDTO userRegisterDTO, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (userRegisterDTO is null || string.IsNullOrWhiteSpace(userRegisterDTO.Username) || string.IsNullOrWhiteSpace(userRegisterDTO.Password))
        {
            return Fail<string>(400, "Username and password must not be empty.");
        }

        if (_users.ContainsKey(userRegisterDTO.Username))
        {
            return Fail<string>(409, "Username already exists.");
        }

        await SetupOrUpdatePasswordAsync(userRegisterDTO.Username, userRegisterDTO.Password);
        return Ok("User registered successfully.");
    }

    /// <summary>
    /// Authenticates a user with the provided username and password. 
    /// If the authentication is successful, it generates a JWT token for the user and returns it in an ApiResponse. 
    /// If the authentication fails, it returns an ApiResponse with an appropriate error message and status code. 
    /// The method also checks for cancellation requests through the provided CancellationToken.
    /// </summary>
    /// <param name="userLoginDTO">The DTO containing the user's login information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An ApiResponse indicating the result of the login attempt.</returns>
    public async Task<ApiResponse<string>> LoginAsync(UserLoginDTO userLoginDTO, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (userLoginDTO is null || string.IsNullOrWhiteSpace(userLoginDTO.Username) || string.IsNullOrWhiteSpace(userLoginDTO.Password))
        {
            return Fail<string>(400, "Username and password must not be empty.");
        }

        var valid = await LoginAsync(userLoginDTO.Username, userLoginDTO.Password, ct);
        if (!valid)
        {
            return Fail<string>(401, "Invalid username or password.");
        }

        var token = _jwtTokenService.GenerateToken(
            userId: userLoginDTO.Username,
            secretKey: _jwtOptions.SecretKey,
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            subject: userLoginDTO.Username,
            expirationInMinutes: _jwtOptions.ExpirationInMinutes
        );

        return Ok(token);
    }

    /// <summary>
    /// Sets up or updates the password for a given username. If the username does not exist, it creates a new user with the provided password. 
    /// The method generates a random salt and a salted hash of the password, 
    /// which are stored in the _users dictionary. 
    /// If the username already exists, it updates the existing credentials with the new password. 
    /// The method throws an ArgumentException if the username or new password is null or empty.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the username or password is null or empty.</exception>
    public async Task SetupOrUpdatePasswordAsync(string username, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("Username and password must not be null or empty.");
        }

        string salt = _hashTokens.GenerateRandomSalt();
        string hash = _hashTokens.GenerateSaltedHash(newPassword, salt);

        _users[username] = new UserCredentials(hash, salt);
    }

    /// <summary>
    /// Initiates a password recovery process for a user identified by their username or email. 
    /// The method generates a unique recovery token, hashes it with a random salt, and stores it in the _recoveryTickets dictionary along with an expiration time and a used flag. 
    /// The raw recovery token is returned in the ApiResponse, which can be sent to the user via email or other means. 
    /// The token is valid for a specified duration (default is 15 minutes). 
    /// If the username or email is not found, the method returns an ApiResponse with an appropriate error message and status code.
    /// </summary>
    /// <param name="requestDTO">The DTO containing the user's password recovery request information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An ApiResponse containing the recovery token or an error message.</returns>
    public async Task<ApiResponse<string>> RequestPasswordRecoveryAsync(PasswordRecoveryRequestDTO requestDTO, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (requestDTO is null || string.IsNullOrWhiteSpace(requestDTO.UsernameOrEmail))
        {
            return Fail<string>(400, "Username or email must not be empty.");
        }

        var token = await RequestPasswordRecoveryAsync(requestDTO.UsernameOrEmail, 15, ct);
        return Ok(token);
    }

    /// <summary>
    /// Confirms a password recovery request by validating the provided recovery token and, if valid, resets the user's password to the new password specified in the confirmDTO. 
    /// The method checks if the confirmDTO is valid and if the recovery token matches the stored token for the user. 
    /// If the token is valid and not expired, it resets the password and marks the recovery ticket as used. 
    /// The method returns an ApiResponse indicating whether the password reset was successful or if there was an error (e.g., invalid token, expired token, or missing information).
    /// </summary>
    /// <param name="confirmDTO">The DTO containing the user's password recovery confirmation information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An ApiResponse indicating the result of the password recovery confirmation.</returns>
    public async Task<ApiResponse<string>> ConfirmPasswordRecoveryAsync(PasswordRecoveryConfirmDTO confirmDTO, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (confirmDTO is null ||
            string.IsNullOrWhiteSpace(confirmDTO.Username) ||
            string.IsNullOrWhiteSpace(confirmDTO.RecoveryToken) ||
            string.IsNullOrWhiteSpace(confirmDTO.NewPassword))
        {
            return Fail<string>(400, "Username, recovery token, and new password must not be empty.");
        }

        var success = await ResetPasswordWithRecoveryAsync(confirmDTO.Username, confirmDTO.RecoveryToken, confirmDTO.NewPassword, ct);
        return success ? Ok("Password reset successful.") : Fail<string>(400, "Invalid recovery token or username.");
    }

    // --------------------
    // Internal async Methods
    // --------------------

    public Task<bool> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!_users.TryGetValue(username, out var credentials))
        {
            return Task.FromResult(false); //* User not found
        }

        bool valid = _hashTokens.VerifyHash(credentials.PasswordHash, password, credentials.Salt);
        return Task.FromResult(valid);
    }

    public Task SetupOrUpdatePasswordAsync(string username, string newPassword, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("Username and password must not be null or empty.");
        }

        string salt = _hashTokens.GenerateRandomSalt();
        string hash = _hashTokens.GenerateSaltedHash(newPassword, salt);

        _users[username] = new UserCredentials(hash, salt);
        return Task.CompletedTask;
    }

    public Task<string> RequestPasswordRecoveryAsync(string usernameOrEmail, int tokenValidityMinutes = 15, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(usernameOrEmail) || !_users.ContainsKey(usernameOrEmail))
        {
            throw new ArgumentException("Username or email not found.");
        }

        string rawToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray());
        string salt = _hashTokens.GenerateRandomSalt();
        string tokenHash = _hashTokens.GenerateSaltedHash(rawToken, salt);

        _recoveryTickets[usernameOrEmail] = new RecoveryTicket(
            TokenHash: tokenHash,
            Salt: salt,
            ExpireUtc: DateTime.UtcNow.AddMinutes(tokenValidityMinutes),
            Used: false
        );

        return Task.FromResult(rawToken);
    }

    /// <summary>
    /// Resets the password for a user using a recovery token. 
    /// The method checks if there is a valid recovery ticket for the given username, 
    /// verifies the provided recovery token against the stored hash, and if valid, 
    /// updates the user's password with the new password.
    /// </summary>
    /// <param name="username">The username of the user whose password is to be reset.</param>
    /// <param name="recoveryToken">The recovery token provided by the user.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A boolean indicating whether the password reset was successful.</returns>
    public async Task<bool> ResetPasswordWithRecoveryAsync(string username, string recoveryToken, string newPassword, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!_recoveryTickets.TryGetValue(username, out var ticket))
        {
            return false; //* No recovery ticket found for user
        }

        if (ticket.Used || DateTime.UtcNow > ticket.ExpireUtc)
        {
            return false; //* Ticket is either used or expired
        }

        bool valid = _hashTokens.VerifyHash(ticket.TokenHash, recoveryToken, ticket.Salt);
        if (!valid)
        {
            return false; //* Invalid recovery token
        }

        await SetupOrUpdatePasswordAsync(username, newPassword);
        _recoveryTickets[username] = ticket with { Used = true }; //* Mark the ticket as used
        return true;
    }

    /// <summary>
    /// Creates a successful ApiResponse with the provided data. The StatusCode is set to 200, Success is set to true, and the Data property contains the provided data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The data to include in the response.</param>
    /// <returns>An ApiResponse containing the provided data.</returns>
    private static ApiResponse<T> Ok<T>(T data) => new()
    {
        Success = true,
        StatusCode = 200,
        Data = data
    };

    /// <summary>
    /// Creates a failed ApiResponse with the provided status code and message. The Success property is set to false, and the Message property contains the provided error message. The Data property is set to null.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="statusCode">The HTTP status code to include in the response.</param>
    /// <param name="message">The error message to include in the response.</param>
    /// <returns>An ApiResponse containing the provided error information.</returns>
    private static ApiResponse<T> Fail<T>(int statusCode, string message) => new()
    {
        Success = false,
        StatusCode = statusCode,
        Message = message
    };

    private sealed record UserCredentials(string PasswordHash, string Salt);
    private sealed record RecoveryTicket(string TokenHash, string Salt, DateTime ExpireUtc, bool Used);
}