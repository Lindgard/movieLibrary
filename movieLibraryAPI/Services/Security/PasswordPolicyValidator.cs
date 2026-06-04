namespace movieLibraryAPI.Services.Security;

public class PasswordPolicyValidator
{
    public bool Validate(string password)
    {
        //* Check for minimum length
        if (password.Length < 8)
        {
            return false;
        }

        //* Check for uppercase letters
        if (!password.Any(char.IsUpper))
        {
            return false;
        }

        //* Check for lowercase letters
        if (!password.Any(char.IsLower))
        {
            return false;
        }

        //* Check for digits
        if (!password.Any(char.IsDigit))
        {
            return false;
        }

        //* Check for special characters
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            return false;
        }

        return true;
    }
}