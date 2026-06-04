using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace movieLibraryAPI.Services.Security;

public class HashTokens
{
    private const int MemorySizeKb = 65536; //* 64 MB
    private const int Iterations = 3;
    private const int HashLength = 32; //* 256 bits
    private const int MinSaltBytes = 16; //* 128 bits

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