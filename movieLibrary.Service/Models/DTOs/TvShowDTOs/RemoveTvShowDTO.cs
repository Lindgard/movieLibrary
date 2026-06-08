using movieLibraryService.Models.Domain;

namespace movieLibraryService.Models.DTOs.TvShowDTOs;

public class RemoveTvShowDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public Genres TvShowGenre { get; set; }
    public string Creator { get; set; } = string.Empty;
}