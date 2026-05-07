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

        if (!string.IsNullOrEmpty(input))
        {
            tvList.Add(new TvShow
            {
                Title = input,
                ReleaseYear = input.Length > 5 ? int.Parse(input.Substring(0, 4)) : 2000, // Just a placeholder for release year
                Description = "Description not provided.",
                Season = input.Length > 5 ? int.Parse(input.Substring(0, 4)) - 2000 : 1, // Placeholder for season based on input length
                TotalEpisodes = input.Length > 5 ? int.Parse(input.Substring(0, 4)) - 2000 + 10 : 10, // Placeholder for total episodes
            });
        }

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