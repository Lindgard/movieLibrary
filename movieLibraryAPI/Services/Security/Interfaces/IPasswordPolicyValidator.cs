namespace movieLibraryAPI.Services.Security.Interfaces;

public interface IPasswordPolicyValidator
{
    bool Validate(string password);
}