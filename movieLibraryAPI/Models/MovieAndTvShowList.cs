using movieLibraryAPI.Models.Domain;

namespace movieLibraryAPI.Models;

/// <summary>
/// This class represents a combined list of movies and TV shows. It contains properties for both movies and TV shows,
/// allowing you to manage them together in a single collection.
/// This can be useful for scenarios where you want to display or manipulate both types of media in a unified way.
/// </summary>
public class MovieAndTvShowList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ICollection<TvShow> Shows { get; set; } = new List<TvShow>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}