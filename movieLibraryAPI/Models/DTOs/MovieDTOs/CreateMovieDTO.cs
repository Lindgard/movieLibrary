using movieLibraryAPI.Models.Domain;

namespace movieLibraryAPI.Models.DTOs.MovieDTOs;

public class CreateMovieDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public Genres MovieGenre { get; set; }
    public string Director { get; set; } = string.Empty;
}