using movieLibraryService.Models.Domain;

namespace movieLibraryService.Models.DTOs.TvShowDTOs;

public class CreateTvShowDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<TvShowSeasonDTO>? Seasons { get; set; }
    public Genres TvShowGenre { get; set; }
    public string Creator { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public TvShowSeasonDTO? Season { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
}