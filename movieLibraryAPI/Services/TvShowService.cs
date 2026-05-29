using movieLibraryAPI.Models.Domain;

namespace movieLibraryAPI.Services;

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
    /// Retrieves all TV shows from the list with optional filtering by 
    /// genre, release year, and title. 
    /// Supports pagination through page number and page size parameters.
    /// </summary>
    /// <param name="genre">Optional genre to filter TV shows.</param>
    /// <param name="releaseYear">Optional release year to filter TV shows.</param>
    /// <param name="title">Optional title to filter TV shows.</param>
    /// <param name="pageNumber">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page for pagination.</param>
    /// <returns>List of TV shows matching the specified criteria.</returns>
    public Task<List<TvShow>> GetAllTvShowsAsync(string? genre = null, int? releaseYear = null, string? title = null, int pageNumber = 1, int pageSize = 10)
    {
        var query = _tvList.AsQueryable();

        if (!string.IsNullOrEmpty(genre) && Enum.TryParse<Genres>(genre, true, out var parsedGenre))
        {
            query = query.Where(s => s.TvShowGenre == parsedGenre);
        }
        if (releaseYear.HasValue)
        {
            query = query.Where(s => s.ReleaseYear == releaseYear.Value);
        }
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(s => s.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        var paginatedTvShows = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Task.FromResult(paginatedTvShows);
    }

    /// <summary>
    /// Adds a new TV show to the list and returns the added TV show.
    /// </summary>
    /// <param name="tvShow">The TV show to add.</param>
    /// <returns>The added TV show.</returns>
    public Task<TvShow> AddTvShowAsync(TvShow tvShow)
    {
        var newShow = new TvShow
        {
            Title = tvShow.Title,
            ReleaseYear = tvShow.ReleaseYear,
            Description = tvShow.Description,
            Seasons = tvShow.Seasons,
            TotalEpisodes = tvShow.TotalEpisodes,
            TvShowGenre = tvShow.TvShowGenre
        };

        _tvList.Add(newShow);
        return Task.FromResult(newShow);
    }

    /// <summary>
    /// Removes a TV show from the list based on the title. Returns the removed TV show if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the TV show to remove.</param>
    /// <returns>The removed TV show if found, otherwise null.</returns>
    public Task<TvShow?> RemoveTvShowAsync(string title)
    {
        var showToRemove = _tvList.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (showToRemove != null)
        {
            _tvList.Remove(showToRemove);
            return Task.FromResult<TvShow?>(showToRemove);
        }
        return Task.FromResult<TvShow?>(null);
    }

    /// <summary>
    /// Updates the details of an existing TV show based on the title. Returns the updated TV show if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the TV show to update.</param>
    /// <param name="updatedShow">Updated TV show details.</param>
    /// <returns>The updated TV show if found, otherwise null.</returns>
    public Task<TvShow?> UpdateTvShowAsync(string title, TvShow updatedShow)
    {
        var showToUpdate = _tvList.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (showToUpdate != null)
        {
            showToUpdate.Title = updatedShow.Title;
            showToUpdate.ReleaseYear = updatedShow.ReleaseYear;
            showToUpdate.Description = updatedShow.Description;
            showToUpdate.Seasons = updatedShow.Seasons;
            showToUpdate.TotalEpisodes = updatedShow.TotalEpisodes;
            showToUpdate.TvShowGenre = updatedShow.TvShowGenre;
        }
        return Task.FromResult(showToUpdate);
    }
}