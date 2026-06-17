using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace movieLibraryService.Services.Security;

public class JwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user ID with the given parameters.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the token is being generated.</param>
    /// <param name="secretKey">The secret key used to sign the token.</param>
    /// <param name="issuer">The issuer of the token.</param>
    /// <param name="audience">The audience for the token.</param>
    /// <param name="subject">The subject of the token.</param>
    /// <param name="expirationInMinutes">The expiration time of the token in minutes.</param>
    /// <returns>The generated JWT token as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the parameters are invalid.</exception>
    public string GenerateToken(
        string userId,
        string secretKey,
        string issuer,
        string audience,
        string subject,
        int expirationInMinutes)
    {
        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        if (string.IsNullOrEmpty(secretKey)) throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));
        if (string.IsNullOrEmpty(issuer)) throw new ArgumentException("Issuer cannot be null or empty.", nameof(issuer));
        if (string.IsNullOrEmpty(audience)) throw new ArgumentException("Audience cannot be null or empty.", nameof(audience));
        if (string.IsNullOrEmpty(subject)) throw new ArgumentException("Subject cannot be null or empty.", nameof(subject));
        if (expirationInMinutes <= 0) throw new ArgumentException("Expiration time must be greater than zero.", nameof(expirationInMinutes));

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        if (keyBytes.Length < 32)
        {
            throw new ArgumentException("Secret key must be at least 32 bytes long.", nameof(secretKey));
        }

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("userId", userId),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var signingKey = new SymmetricSecurityKey(keyBytes);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expirationInMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a JWT token and returns the associated ClaimsPrincipal if valid, or null if invalid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <param name="secretKey">The secret key used to validate the token's signature.</param>
    /// <param name="issuer">The expected issuer of the token.</param>
    /// <param name="audience">The expected audience of the token.</param>
    /// <param name="validateLifetime">Whether to validate the token's expiration.</param>
    /// <returns>The ClaimsPrincipal if the token is valid; otherwise, null.</returns>
    public ClaimsPrincipal? ValidateToken(
        string token,
        string secretKey,
        string issuer,
        string audience,
        bool validateLifetime = true)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        if (string.IsNullOrWhiteSpace(secretKey)) return null;

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        if (keyBytes.Length < 32) return null;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extracts the user ID from a valid JWT token. Returns null if the token is invalid or does not contain a user ID claim.
    /// </summary>
    /// <param name="token">The JWT token from which to extract the user ID.</param>
    /// <param name="secretKey">The secret key used to validate the token's signature.</param>
    /// <param name="issuer">The expected issuer of the token.</param>
    /// <param name="audience">The expected audience of the token.</param>
    /// <returns>The user ID if the token is valid and contains a user ID claim; otherwise, null.</returns>
    public string? GetUserIdFromToken(string token, string secretKey, string issuer, string audience)
    {
        var principal = ValidateToken(token, secretKey, issuer, audience);
        return principal?.FindFirst("userId")?.Value
            ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}