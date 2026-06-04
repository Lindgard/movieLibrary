using movieLibraryAPI.Models.Domain.Users;
using System.Collections.Concurrent;

namespace movieLibraryAPI.Services.Security;

public class LoginService
{
    private readonly HashTokens _hashTokens = new();

    private readonly ConcurrentDictionary<string, UserCredentials> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, RecoveryTicket> _recoveryTickets = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes the LoginService with a default admin user. 
    /// The default credentials are "admin" for the username and "password" for the password.
    /// </summary>
    public LoginService()
    {
        SetupOrUpdatePassword("admin", "password");
    }

    /// <summary>
    /// Sets up or updates the password for a given username. 
    /// If the user does not exist, it creates a new user with the provided credentials.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>True if the login is successful; otherwise, false.</returns>
    public bool Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (!_users.TryGetValue(username, out var user))
        {
            return false;
        }

        return _hashTokens.ValidateToken(user.PasswordHash, password, user.Salt);
    }

    /// <summary>
    /// Sets up or updates the password for a given username. 
    /// If the user does not exist, it creates a new user with the provided credentials.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <exception cref="ArgumentException">Thrown when the username or password is null or empty.</exception>
    public void SetupOrUpdatePassword(string username, string newPassword)
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
    /// Generates a password recovery token for the specified username. 
    /// The token is valid for a specified time-to-live (TTL) in minutes, which defaults to 15 minutes. 
    /// The method returns the raw token that can be sent to the user for password recovery. 
    /// If the username is not found, an ArgumentException is thrown.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="ttlMinutes">The time-to-live (TTL) for the recovery token in minutes.</param>
    /// <returns>The raw recovery token that can be sent to the user.</returns>
    /// <exception cref="ArgumentException">Thrown when the username is not found.</exception>
    public string RequestPasswordRecovery(string username, int ttlMinutes = 15)
    {
        if (string.IsNullOrWhiteSpace(username) || !_users.ContainsKey(username))
        {
            throw new ArgumentException("User not found.");
        }

        string rawToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray());
        string salt = _hashTokens.GenerateRandomSalt();
        string tokenHash = _hashTokens.GenerateSaltedHash(rawToken, salt);

        _recoveryTickets[username] = new RecoveryTicket(
            TokenHash: tokenHash,
            Salt: salt,
            ExpireUtc: DateTime.UtcNow.AddMinutes(ttlMinutes),
            Used: false
        );
        return rawToken;
    }

    /// <summary>
    /// Resets the password for a user using a recovery token. 
    /// The method validates the provided recovery token against the stored token hash and salt for the user. 
    /// If the token is valid and has not expired or been used, the user's password is updated with the new password, and the recovery ticket is marked as used. 
    /// The method returns true if the password reset is successful; otherwise, it returns false.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="recoveryToken">The recovery token provided by the user.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <returns>True if the password reset is successful; otherwise, false.</returns>
    public bool ResetPasswordWithRecovery(string username, string recoveryToken, string newPassword)
    {
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

        SetupOrUpdatePassword(username, newPassword);
        _recoveryTickets[username] = ticket with { Used = true }; //* Mark the ticket as used
        return true;
    }

    private sealed record UserCredentials(string PasswordHash, string Salt);
    private sealed record RecoveryTicket(string TokenHash, string Salt, DateTime ExpireUtc, bool Used);
}