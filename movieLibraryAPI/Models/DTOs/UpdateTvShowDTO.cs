using movieLibrary.Models.Domain;
namespace movieLibrary.Models.TvShows;

public class UpdateTvShowDTO
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public Genres TvShowGenre { get; set; }
    public string Creator { get; set; } = string.Empty;
    public int Season { get; set; }
}
