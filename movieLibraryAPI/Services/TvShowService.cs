using movieLibrary.Models.TvShows;

namespace movieLibrary.Services;

public class TvShowService
{
    /// <summary>
    /// In-memory list to store TV shows. This simulates a database for the purpose of this example.
    /// </summary>
    private readonly List<TvShow> _tvList = new List<TvShow>();

    public TvShowService()
    {
    }

    /// <summary>
    /// Adds a new TV show to the list and returns the added TV show.
    /// </summary>
    /// <param name="tvShow">The TV show to add.</param>
    /// <returns>The added TV show.</returns>
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

    /// <summary>
    /// Removes a TV show from the list based on the title. Returns the removed TV show if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the TV show to remove.</param>
    /// <returns>The removed TV show if found, otherwise null.</returns>
    public TvShow? RemoveTvShow(string title)
    {
        var showToRemove = _tvList.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (showToRemove != null)
        {
            _tvList.Remove(showToRemove);
            return showToRemove;
        }
        return null;
    }

    /// <summary>
    /// Updates the details of an existing TV show based on the title. Returns the updated TV show if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the TV show to update.</param>
    /// <param name="updatedShow">Updated TV show details.</param>
    /// <returns>The updated TV show if found, otherwise null.</returns>
    public TvShow? UpdateTvShow(string title, TvShow updatedShow)
    {
        var showToUpdate = _tvList.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (showToUpdate != null)
        {
            showToUpdate.Title = updatedShow.Title;
            showToUpdate.ReleaseYear = updatedShow.ReleaseYear;
            showToUpdate.Description = updatedShow.Description;
            showToUpdate.Season = updatedShow.Season;
            showToUpdate.TotalEpisodes = updatedShow.TotalEpisodes;
            showToUpdate.TvShowGenre = updatedShow.TvShowGenre;
        }
        return showToUpdate;
    }

    /// <summary>
    /// Retrieves all TV shows from the list.
    /// </summary>
    /// <returns>List of all TV shows.</returns>
    public List<TvShow> GetAllTvShows()
    {
        return _tvList;
    }
}