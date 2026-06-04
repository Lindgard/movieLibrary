namespace movieLibraryAPI.Models.DTOs.UserDTOs;

public class PasswordRecoveryConfirmDTO
{
    public string Username { get; set; } = string.Empty;
    public string RecoveryToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}