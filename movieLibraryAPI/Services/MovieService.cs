using movieLibrary.Models.Movies;

namespace movieLibrary.Services;

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

    public Movie AddMovie(Movie movie)
    {
        var newMovie = new Movie
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            MovieGenre = movie.MovieGenre
        };
        _movieList.Add(newMovie);
        return newMovie;
    }

    /// <summary>
    /// Removes a movie from the list based on the title. Returns the removed movie if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the movie to remove.</param>
    /// <returns>The removed movie if found, otherwise null.</returns>
    public Movie? RemoveMovie(string title)
    {
        var movieToRemove = _movieList.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (movieToRemove != null)
        {
            _movieList.Remove(movieToRemove);
            return movieToRemove;
        }
        return null;
    }

    /// <summary>
    /// Updates the details of an existing movie based on the title. Returns the updated movie if found, otherwise returns null.
    /// </summary>
    /// <param name="title">Title of the movie to update.</param>
    /// <param name="updatedMovie">Movie object containing the updated details.</param>
    /// <returns>The updated movie if found, otherwise null.</returns>
    public Movie? UpdateMovie(string title, Movie updatedMovie)
    {
        var movieToUpdate = _movieList.FirstOrDefault(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (movieToUpdate != null)
        {
            movieToUpdate.Title = updatedMovie.Title;
            movieToUpdate.ReleaseYear = updatedMovie.ReleaseYear;
            movieToUpdate.Description = updatedMovie.Description;
            movieToUpdate.MovieGenre = updatedMovie.MovieGenre;
            movieToUpdate.Director = updatedMovie.Director;
            return movieToUpdate;
        }
        return null;
    }

    /// <summary>
    /// Retrieves all movies from the list.
    /// </summary>
    /// <returns>List of all movies.</returns>
    public List<Movie> GetAllMovies()
    {
        return _movieList;
    }
}