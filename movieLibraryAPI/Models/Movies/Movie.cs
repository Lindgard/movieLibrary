namespace movieLibrary.Models.Movies;

public class Movie
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public Genres MovieGenre { get; set; }

    /// <summary>
    /// Initializes a new empty instance of the Movie class with default values.
    /// </summary>
    public Movie()
    {

    }

    /// <summary>
    /// Initializes a new instance of the Movie class with the specified details.
    /// </summary>
    /// <param name="title">The title of the movie.</param>
    /// <param name="releaseYear">The release year of the movie.</param>
    /// <param name="description">A brief description of the movie.</param>
    /// <param name="director">The director of the movie.</param>
    /// <param name="genres">The genre of the movie.</param>
    public Movie(string title, int releaseYear, string description, string director, Genres genres)
    {
        Title = title;
        ReleaseYear = releaseYear;
        Description = description;
        Director = director;
        MovieGenre = genres;
    }
}