using movieLibrary.Models;

namespace movieLibrary.Services;

public class TvShowService
{
    private List<TvShow> tvList = new List<TvShow>();

    public TvShowService()
    {
        AddTvShow();
    }

    public void AddTvShow()
    {
        //TODO create method to add new show to list
        var input = Console.ReadLine()?.Trim();
        //* make use of input to let user add a TV-show with a title
        //* and let them add other information later


        //* Added a tv-show for testing purposes
        tvList.Add(new TvShow
        {
            Title = "The X-Files",
            ReleaseYear = 1993,
            Description = "A pair of FBI agents investigate paranormal phenomena and unsolved cases, often involving extraterrestrial life.",
            Season = 1,
            TotalEpisodes = 24,
            EpisodeName = "Pilot",
            EpisodeNumber = 01,
            TvShowGenre = Genres.SciFi
        });
    }
}