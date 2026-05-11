using movieLibrary.Models.Domain;
namespace movieLibrary.Models.DTOs;

public class UpdateMovieDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public Genres MovieGenre { get; set; }
    public string Director { get; set; } = string.Empty;
}