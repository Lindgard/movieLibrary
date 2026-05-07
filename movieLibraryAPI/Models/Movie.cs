namespace movieLibrary.Models;

public class Movie
{
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public Genres MovieGenre { get; set; }

    public Movie()
    {

    }

    /// <summary>
    /// Constructor that takes in parameters for all properties.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="releaseYear"></param>
    /// <param name="description"></param>
    /// <param name="director"></param>
    /// <param name="genres"></param>
    public Movie(string title, int releaseYear, string description, string director, Genres genres)
    {
        Title = title;
        ReleaseYear = releaseYear;
        Description = description;
        Director = director;
        MovieGenre = genres;
    }
}