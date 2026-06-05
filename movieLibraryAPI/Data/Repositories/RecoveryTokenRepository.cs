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

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }
        return email.Trim().ToLowerInvariant();
    }

    private static string GenerateSecret()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

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