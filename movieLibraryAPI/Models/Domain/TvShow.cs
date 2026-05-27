using movieLibrary.Models.Interfaces;

namespace movieLibrary.Models.Domain;

public class TvShow : ITvShow
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public Season? Season { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public string Creator { get; set; } = string.Empty;
    public Genres TvShowGenre { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
}