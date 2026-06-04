namespace movieLibraryAPI.Models.DTOs.UserDTOs;

public class PasswordRecoveryRequestDTO
{
    public string UsernameOrEmail { get; set; } = string.Empty;
}