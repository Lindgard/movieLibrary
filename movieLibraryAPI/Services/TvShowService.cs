using movieLibrary.Models.TvShows;

namespace movieLibrary.Services;

public class TvShowService
{
    private readonly List<TvShow> _tvList = new List<TvShow>();

    public TvShowService()
    {
    }

    public TvShow AddTvShow(TvShow tvShow)
    {
        var newShow = new TvShow
        {
            Title = tvShow.Title,
            ReleaseYear = tvShow.ReleaseYear,
            Description = tvShow.Description,
            Season = tvShow.Season,
            TotalEpisodes = tvShow.TotalEpisodes,
            TvShowGenre = tvShow.TvShowGenre
        };

        _tvList.Add(newShow);
        return newShow;
    }
}