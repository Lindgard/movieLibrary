namespace movieLibraryService.Services.Security.Interfaces;

public interface IPasswordPolicyValidator
{
    bool ValidatePassword(string password);
}