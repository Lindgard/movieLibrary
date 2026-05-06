namespace movieLibrary.Models;

public class TvShow
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int Season { get; set; }
    public string Description { get; set; } = string.Empty;
    public string EpisodeName { get; set; } = string.Empty;
    public int EpisodeNumber { get; set; }
    public Genres TvShowGenre { get; set; }
}