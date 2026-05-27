using movieLibraryAPI.Models.Domain;

namespace movieLibraryAPI.Services;

public class MovieService
{
    /// <summary>
    /// In-memory list to store movies. This simulates a database for the purpose of this example.
    /// </summary>
    private readonly List<Movie> _movieList = new List<Movie>();

    /// <summary>
    /// Initializes a new instance of the MovieService class.
    /// </summary>
    public MovieService()
    {
    }

    /// <summary>
    /// Adds a new movie to the list and returns the added movie. 
    /// This method simulates adding a movie to a database or another data source.
    /// </summary>
    /// <param name="movie">The movie to add.</param>
    /// <returns>The added movie.</returns>
    public async Task<Movie> AddMovieAsync(Movie movie)
    {
        var newMovie = new Movie
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            MovieGenre = movie.MovieGenre
        };
        _movieList.Add(newMovie);
        return await Task.FromResult(newMovie);
    }

    /// <summary>
    /// Removes a movie from the list based on the title. Returns the removed movie if found, otherwise returns null.
    /// This method simulates removing a movie from a database or another data source.
    /// </summary>
    /// <param name="title">Title of the movie to remove.</param>
    /// <returns>The removed movie if found, otherwise null.</returns>
    public async Task<Movie?> RemoveMovieAsync(string title)
    {
        var movieToRemove = _movieList.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (movieToRemove != null)
        {
            _movieList.Remove(movieToRemove);
            return await Task.FromResult(movieToRemove);
        }
        return await Task.FromResult<Movie?>(null);
    }

    /// <summary>
    /// Updates the details of an existing movie based on the title. Returns the updated movie if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the movie to update.</param>
    /// <param name="updatedMovie">Movie object containing the updated details.</param>
    /// <returns>The updated movie if found, otherwise null.</returns>
    public async Task<Movie?> UpdateMovieAsync(string title, Movie updatedMovie)
    {
        var movieToUpdate = _movieList.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (movieToUpdate != null)
        {
            movieToUpdate.Title = updatedMovie.Title;
            movieToUpdate.ReleaseYear = updatedMovie.ReleaseYear;
            movieToUpdate.Description = updatedMovie.Description;
            movieToUpdate.MovieGenre = updatedMovie.MovieGenre;
            movieToUpdate.Director = updatedMovie.Director;
            return await Task.FromResult(movieToUpdate);
        }
        return await Task.FromResult<Movie?>(null);
    }

    /// <summary>
    /// Retrieves all movies from the list.
    /// </summary>
    /// <returns>List of all movies.</returns>
    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        return await Task.FromResult(_movieList);
    }
}