using movieLibrary.Models;

namespace movieLibrary.Services;

public class TvShowService
{
    private List<TvShow> tvList = new List<TvShow>();

    public TvShowService()
    {
        //* Added a tv-show for testing purposes
        tvList.Add(new TvShow
        {
            Title = "The X-Files",
            ReleaseYear = 1993,
            Description = "A pair of FBI agents investigate paranormal phenomena and unsolved cases, often involving extraterrestrial life.",
            Season = 1,
            EpisodeName = "Pilot",
            EpisodeNumber = 01,
            TvShowGenre = Genres.SciFi
        });
    }
}