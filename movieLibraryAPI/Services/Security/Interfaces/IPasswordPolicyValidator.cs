namespace movieLibraryAPI.Services.Security.Interfaces;

public interface IPasswordPolicyValidator
{
    bool ValidatePassword(string password);
}