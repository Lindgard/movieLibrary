namespace movieLibraryAPI.Models.DTOs.UserDTOs;

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}