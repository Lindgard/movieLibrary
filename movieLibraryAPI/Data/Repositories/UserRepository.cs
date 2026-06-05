using movieLibraryAPI.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using movieLibraryAPI.Models.Domain;
using movieLibraryAPI.Services.Security;
using movieLibraryAPI.Services.Security.Interfaces;

namespace movieLibraryAPI.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MovieLibraryDbContext _dbContext;
    private readonly HashTokens _hashTokens;
    private readonly IPasswordPolicyValidator _passwordPolicyValidator;

    /// <summary>
    /// Initializes a new instance of the UserRepository class 
    /// with the specified database context and hash token service.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    /// <param name="hashTokens">The hash token service to be used for password hashing.</param>
    /// <param name="passwordPolicyValidator">The password policy validator to be used for validating passwords.</param>
    public UserRepository(
        MovieLibraryDbContext dbContext,
        HashTokens hashTokens,
        IPasswordPolicyValidator passwordPolicyValidator)
    {
        _dbContext = dbContext;
        _hashTokens = hashTokens;
        _passwordPolicyValidator = passwordPolicyValidator;
    }

    /// <summary>
    /// Creates a new user with the specified email and password.
    /// </summary>
    /// <param name="email">The email of the new user.</param>
    /// <param name="password">The password of the new user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a user with the specified email already exists.</exception>
    public async Task CreateUserAsync(string email, string password)
    {
        string normalizedEmail = NormalizeEmail(email);

        bool exists = await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail);
        if (exists)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        string salt = _hashTokens.GenerateRandomSalt();
        string passwordHash = _hashTokens.GenerateSaltedHash(password, salt);

        string username = BuildUsernameFromEmail(normalizedEmail);
        bool usernameTaken = await _dbContext.Set<User>().AnyAsync(u => u.Username == username);
        if (usernameTaken)
        {
            username = $"{username}_{Guid.NewGuid():N}"[..Math.Min(username.Length + 7, 32)];
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = normalizedEmail,
            Username = username,
            PasswordHash = passwordHash,
            PasswordSalt = salt,
            CreatedAtUtc = DateTime.UtcNow,
            PasswordUpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Set<User>().Add(user);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves the password hash for the user with the specified email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The password hash of the user.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email is not found.</exception>
    public async Task<string> GetPasswordHashAsync(string email)
    {
        string normalizedEmail = NormalizeEmail(email);

        var user = await _dbContext.Set<User>()
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return user.PasswordHash;

    }

    /// <summary>
    /// Retrieves the user ID for the user with the specified email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The user ID of the user.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email is not found.</exception>
    public async Task<string> GetUserIdAsync(string email)
    {
        string normalizedEmail = NormalizeEmail(email);

        var user = await _dbContext.Set<User>()
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return user.UserId.ToString();
    }

    /// <summary>
    /// Updates the password for the user with the specified email.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="newPassword">The new password for the user.</param>
    /// <returns>The new password hash of the user.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email is not found.</exception>
    public async Task<string> UpdatePasswordAsync(string email, string newPassword)
    {
        string normalizedEmail = NormalizeEmail(email);
        EnsureValidPassword(newPassword);

        var user = await _dbContext.Set<User>()
            .SingleOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        string newSalt = _hashTokens.GenerateRandomSalt();
        string newPasswordHash = _hashTokens.GenerateSaltedHash(newPassword, newSalt);

        user.PasswordSalt = newSalt;
        user.PasswordHash = newPasswordHash;
        user.PasswordUpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return newPasswordHash;
    }

    /// <summary>
    /// Updates the user information for the user with the specified email.
    /// </summary>
    /// <param name="email">The current email of the user.</param>
    /// <param name="newEmail">The new email of the user.</param>
    /// <param name="newPassword">The new password of the user.</param>
    /// <returns>The user ID of the updated user.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the user with the specified email is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the new email is already in use by another account.</exception>
    public async Task<string> UpdateUserAsync(string email, string newEmail, string newPassword)
    {
        string currentEmail = NormalizeEmail(email);
        string updatedEmail = NormalizeEmail(newEmail);
        EnsureValidPassword(newPassword);

        var user = await _dbContext.Set<User>()
            .SingleOrDefaultAsync(u => u.Email == currentEmail);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (currentEmail != updatedEmail)
        {
            bool emailInUse = await _dbContext.Set<User>().AnyAsync(u => u.Email == updatedEmail);
            if (emailInUse)
            {
                throw new InvalidOperationException("The new email is already in use by another account.");
            }

            user.Email = updatedEmail;
        }

        string newSalt = _hashTokens.GenerateRandomSalt();
        string newPasswordHash = _hashTokens.GenerateSaltedHash(newPassword, newSalt);

        user.PasswordSalt = newSalt;
        user.PasswordHash = newPasswordHash;
        user.PasswordUpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return user.UserId.ToString();
    }

    /// <summary>
    /// Checks if a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    public async Task<bool> UserExistsAsync(string email)
    {
        string normalizedEmail = NormalizeEmail(email);
        return await _dbContext.Set<User>().AnyAsync(u => u.Email == normalizedEmail);
    }

    /// <summary>
    /// Normalizes the email by trimming whitespace and converting it to lowercase.
    /// </summary>
    /// <param name="email">The email to normalize.</param>
    /// <returns>The normalized email.</returns>
    /// <exception cref="ArgumentException">Thrown when the email is null or empty.</exception>
    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        return email.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Builds a username from the specified email by extracting the part before the "@" symbol.
    /// </summary>
    /// <param name="email">The email to extract the username from.</param>
    /// <returns>The username extracted from the email.</returns>
    private static string BuildUsernameFromEmail(string email)
    {
        string usernamePart = email.Split("@")[0].Trim();
        return string.IsNullOrWhiteSpace(usernamePart) ? "user" : usernamePart;
    }

    /// <summary>
    /// Ensures that the provided password meets the required policy.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the password does not meet the required policy.</exception>
    private void EnsureValidPassword(string password)
    {
        if (!_passwordPolicyValidator.ValidatePassword(password))
        {
            throw new ArgumentException("The provided password does not meet the required policy.", nameof(password));
        }
    }
}