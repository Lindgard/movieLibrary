namespace movieLibraryAPI.Data.Repositories.Interfaces;

public interface IRecoveryTokenRepository
{
    Task<string> CreateRecoveryTokenAsync(string email);
    Task<bool> ValidateRecoveryTokenAsync(string token);
    Task InvalidateRecoveryTokenAsync(string token);
}