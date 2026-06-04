using System.Security.Cryptography;
using System.Text;

namespace movieLibraryAPI.Services.Security;

public class HashTokens
{
    private const int MemorySizeKb = 65536; //* 64 MB
    private const int Iterations = 3;
    private const int HashLength = 32; //* 256 bits
    private const int MinSaltBytes = 16; //* 128 bits

    public bool ValidateToken(string token, string rawData, string salt)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(rawData) || string.IsNullOrEmpty(salt)) return false;

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
}