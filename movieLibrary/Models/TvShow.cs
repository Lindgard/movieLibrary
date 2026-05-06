using movieLibrary.Models;

namespace movieLibrary.Models;

public class TvShow
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string EpisodeName { get; set; } = string.Empty;
    public Genres TvShowGenre { get; set; }
}