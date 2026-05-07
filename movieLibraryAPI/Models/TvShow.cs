using movieLibrary.Models.Interfaces;

namespace movieLibrary.Models;

public class TvShow : ITvShow
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public List<TvShowEpisode> Season { get; set; } = new List<TvShowEpisode>();
    public string Description { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public Genres TvShowGenre { get; set; }
}