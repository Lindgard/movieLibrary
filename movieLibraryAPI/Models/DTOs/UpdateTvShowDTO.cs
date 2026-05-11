using movieLibrary.Models.Domain;
namespace movieLibrary.Models.DTOs;

public class UpdateTvShowDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public Genres TvShowGenre { get; set; }
    public string Creator { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public TvShowSeasonDTO? Season { get; set; }
}
