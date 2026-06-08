using Microsoft.EntityFrameworkCore;
using movieLibraryAPI.Data.Repositories.Interfaces;
using movieLibraryAPI.Services.Security;
using System.Security.Cryptography;
using movieLibraryAPI.Models.Domain;

namespace movieLibraryAPI.Data.Repositories;

public class RecoveryTokenRepository : IRecoveryTokenRepository
{
    private readonly MovieLibraryDbContext _context;
    private readonly HashTokens _hashTokens;
    private static readonly TimeSpan TokenTtl = TimeSpan.FromMinutes(15);

    public RecoveryTokenRepository(MovieLibraryDbContext context, HashTokens hashTokens)
    {
        _context = context;
        _hashTokens = hashTokens;
    }

    /// <summary>
    /// Generates a new recovery token for the user associated with the provided email.
    /// </summary>
    /// <param name="email">The email of the user for whom to generate the recovery token.</param>
    /// <returns>The generated recovery token.</returns>
    /// <exception cref="ArgumentException">Thrown if no user is found with the provided email.</exception>
    public async Task<string> CreateRecoveryTokenAsync(string email)
    {
        string normalizedEmail = NormalizeEmail(email);

        User? user = await _context.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user is null)
        {
            throw new ArgumentException("No user found with the provided email.");
        }

        DateTime now = DateTime.UtcNow;

        List<RecoveryToken> activeTokens = await _context.RecoveryTokens
            .Where(t =>
                t.UserId == user.UserId &&
                t.InvalidatedAtUtc == null &&
                t.UsedAtUtc == null &&
                t.ExpiresAtUtc > now)
            .ToListAsync();

        foreach (var t in activeTokens)
        {
            t.InvalidatedAtUtc = now;
        }

        string secret = GenerateSecret();
        string salt = _hashTokens.GenerateRandomSalt();
        string hash = _hashTokens.GenerateSaltedHash(secret, salt);

        var tokenEntity = new RecoveryToken
        {
            RecoveryTokenId = Guid.NewGuid(),
            UserId = user.UserId,
            TokenHash = hash,
            TokenSalt = salt,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.Add(TokenTtl),
            InvalidatedAtUtc = null,
            UsedAtUtc = null
        };

        _context.RecoveryTokens.Add(tokenEntity);
        await _context.SaveChangesAsync();

        return $"{tokenEntity.RecoveryTokenId:N}.{secret}";
    }

    /// <summary>
    /// Invalidates the specified recovery token, preventing any future validation from succeeding.
    /// </summary>
    /// <param name="token">The recovery token to invalidate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvalidateRecoveryTokenAsync(string token)
    {
        if (!TryParseToken(token, out Guid tokenId, out _))
        {
            return;
        }

        RecoveryToken? tokenEntity = await _context.RecoveryTokens
            .SingleOrDefaultAsync(t => t.RecoveryTokenId == tokenId);

        if (tokenEntity is null || tokenEntity.InvalidatedAtUtc != null)
        {
            return;
        }

        tokenEntity.InvalidatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Validates the specified recovery token, checking if it is still valid and has not been used or invalidated.
    /// </summary>
    /// <param name="token">The recovery token to validate.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the token is valid.</returns>
    public async Task<bool> ValidateRecoveryTokenAsync(string token)
    {
        if (!TryParseToken(token, out Guid tokenId, out string secret))
        {
            return false;
        }

        DateTime no = DateTime.UtcNow;

        RecoveryToken? tokenEntity = await _context.RecoveryTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.RecoveryTokenId == tokenId);

        if (tokenEntity is null)
        {
            return false;
        }

        if (tokenEntity.InvalidatedAtUtc != null || tokenEntity.UsedAtUtc != null || tokenEntity.ExpiresAtUtc <= no)
        {
            return false;
        }

        return _hashTokens.VerifyHash(tokenEntity.TokenHash, secret, tokenEntity.TokenSalt);
    }

    /// <summary>
    /// Marks the token as used, preventing any future validation from succeeding. 
    /// This should be called after a successful password reset to ensure the token cannot be reused.
    /// </summary>
    /// <param name="token">The recovery token to mark as used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task MarkTokenAsUsedAsync(string token)
    {
        if (!TryParseToken(token, out Guid tokenId, out _))
        {
            return;
        }

        RecoveryToken? tokenEntity = await _context.RecoveryTokens
            .SingleOrDefaultAsync(t => t.RecoveryTokenId == tokenId);

        if (tokenEntity is null || tokenEntity.UsedAtUtc != null)
        {
            return;
        }

        tokenEntity.UsedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Normalizes the provided email by trimming whitespace and converting it to lowercase.
    /// </summary>
    /// <param name="email">The email address to normalize.</param>
    /// <returns>The normalized email address.</returns>
    /// <exception cref="ArgumentException">Thrown if the email is null or empty.</exception>
    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }
        return email.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Generates a secure random secret for use in recovery tokens. 
    /// The secret is a 64-character hexadecimal string (32 bytes).
    /// </summary>
    /// <returns></returns>
    private static string GenerateSecret()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    /// <summary>
    /// Attempts to parse the provided token string into its components: the token ID and the secret.
    /// </summary>
    /// <param name="token">The token string to parse.</param>
    /// <param name="tokenId">The parsed token ID.</param>
    /// <param name="secret">The parsed secret.</param>
    /// <returns>True if the token was successfully parsed; otherwise, false.</returns>
    private static bool TryParseToken(string token, out Guid tokenId, out string secret)
    {
        tokenId = Guid.Empty;
        secret = string.Empty;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        string[] parts = token.Split('.', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[1]))
        {
            return false;
        }

        if (!Guid.TryParse(parts[0], out tokenId))
        {
            return false;
        }

        secret = parts[1];
        return true;
    }
}