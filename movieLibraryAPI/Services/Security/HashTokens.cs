using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace movieLibraryAPI.Services.Security;

public class HashTokens
{
    /// <summary>
    /// Configuration parameters for Argon2id hashing. 
    /// MemorySize is set to 64 MB, Iterations to 3, and HashLength to 32 bytes (256 bits). 
    /// The minimum salt length is set to 16 bytes (128 bits) to ensure sufficient randomness.
    /// </summary>
    private const int MemorySizeKb = 65536; //* 64 MB
    private const int Iterations = 3;
    private const int HashLength = 32; //* 256 bits
    private const int MinSaltBytes = 16; //* 128 bits

    /// <summary>
    /// Validates a provided token against the expected hash of the raw data and salt.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <param name="rawData">The raw data used to generate the expected hash.</param>
    /// <param name="salt">The salt used in the hashing process.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    public bool ValidateToken(string token, string rawData, string salt)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(rawData) || string.IsNullOrEmpty(salt))
        {
            return false;
        }

        string expectedToken = GenerateSaltedHash(rawData, salt);

        try
        {
            byte[] provided = Convert.FromHexString(token);
            byte[] expected = Convert.FromHexString(expectedToken);

            return provided.Length == expected.Length &&
                CryptographicOperations.FixedTimeEquals(provided, expected);
        }
        catch (FormatException)
        {
            return false; //* Invalid hex string
        }
    }

    /// <summary>
    /// Generates a salted hash for the given raw data and salt using Argon2id.
    /// </summary>
    /// <param name="rawData">The raw data to hash.</param>
    /// <param name="salt">The salt to use in the hashing process.</param>
    /// <returns>The generated salted hash as a hexadecimal string.</returns>
    /// <exception cref="ArgumentException">Thrown when rawData or salt is null or empty, or when the salt is too short.</exception>
    public string GenerateSaltedHash(string rawData, string salt)
    {
        if (string.IsNullOrEmpty(rawData) || string.IsNullOrEmpty(salt))
        {
            throw new ArgumentException("Raw data and salt must not be null or empty.");
        }

        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        if (saltBytes.Length < MinSaltBytes)
        {
            throw new ArgumentException($"Salt must be at least {MinSaltBytes} bytes long.");
        }

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(rawData))
        {
            Salt = saltBytes,
            MemorySize = MemorySizeKb,
            Iterations = Iterations,
            DegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
        };

        byte[] hashBytes = argon2.GetBytes(HashLength);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Verifies that the provided raw data and salt produce a hash that matches the stored hash.
    /// </summary>
    /// <param name="storedHashHex">The stored hash in hexadecimal format.</param>
    /// <param name="rawData">The raw data to verify.</param>
    /// <param name="salt">The salt used in the hashing process.</param>
    /// <returns>True if the hash matches; otherwise, false.</returns>
    public bool VerifyHash(string storedHashHex, string rawData, string salt)
    => ValidateToken(storedHashHex, rawData, salt);

    /// <summary>
    /// Generates a random salt of the specified length.
    /// </summary>
    /// <param name="length">The length of the salt in bytes. Must be at least <see cref="MinSaltBytes"/>.</param>
    /// <returns>The generated salt as a hexadecimal string.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified length is less than <see cref="MinSaltBytes"/>.</exception>
    public string GenerateRandomSalt(int length = MinSaltBytes)
    {
        if (length < MinSaltBytes)
        {
            throw new ArgumentException($"Salt length must be at least {MinSaltBytes} bytes.");
        }

        byte[] saltBytes = new byte[length];
        RandomNumberGenerator.Fill(saltBytes);
        return Convert.ToHexString(saltBytes);
    }

    /// <summary>
    /// Converts a salt string to a byte array. Prefers hex salt from <see cref="GenerateRandomSalt"/>, but falls back to UTF-8 bytes if not valid hex.
    /// </summary>
    /// <param name="salt">The salt string to convert.</param>
    /// <returns>The salt as a byte array.</returns>
    private static byte[] GetSaltBytes(string salt)
    {
        //* Prefer hex salt from GenerateRandomSalt, but fallback to UTF-8 bytes if not valid hex
        try
        {
            return Convert.FromHexString(salt);
        }
        catch (FormatException)
        {
            return Encoding.UTF8.GetBytes(salt);
        }
    }
}