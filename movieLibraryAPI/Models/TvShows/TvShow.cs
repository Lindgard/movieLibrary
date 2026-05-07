using movieLibrary.Models.Interfaces;

namespace movieLibrary.Models.TvShows;

public class TvShow : ITvShow
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public TvShowSeasonDTO? Season { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public Genres TvShowGenre { get; set; }
}