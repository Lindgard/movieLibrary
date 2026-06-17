using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace movieLibraryService.Services.Security;

public class JwtTokenService
{
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
        if (keyBytes.Length < 16)
        {
            throw new ArgumentException("Secret key must be at least 16 bytes long.", nameof(secretKey));
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
}