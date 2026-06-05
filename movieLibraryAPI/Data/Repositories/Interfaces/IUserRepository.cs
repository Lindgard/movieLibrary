namespace movieLibraryAPI.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string email);
    Task CreateUserAsync(string email, string password);
    Task<string> GetPasswordHashAsync(string email);
    Task<string> GetUserIdAsync(string email);
    Task<string> UpdatePasswordAsync(string email, string newPassword);
    Task<string> UpdateUserAsync(string email, string newEmail, string newPassword);
}