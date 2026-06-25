using movieLibraryService.Models.Domain;
using movieLibraryService.Models.DTOs.MovieDTOs;

namespace movieLibraryAPI.Mappings;

public static class MovieMappings
{
    /// <summary>
    /// Converts a Movie domain model to a MovieDTO. This method takes a Movie object and maps its properties to a new MovieDTO object, 
    /// which is a simplified version of the Movie used for data transfer purposes.
    /// </summary>
    /// <param name="movie">The Movie domain model to be converted.</param>
    /// <returns>A MovieDTO object containing the mapped properties from the Movie domain model.</returns>
    public static MovieDTO ToDto(this Movie movie)
    {
        return new MovieDTO
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            Director = movie.Director,
            MovieGenre = movie.MovieGenre,
            Id = movie.Id
        };
    }

    /// <summary>
    /// Converts a Movie domain model to an UpdateMovieDTO. This method takes a Movie object and maps its properties to a new UpdateMovieDTO object,
    /// which is used for updating movie information.
    /// </summary>
    /// <param name="movie">The Movie domain model to be converted.</param>
    /// <returns>An UpdateMovieDTO object containing the mapped properties from the Movie domain model.</returns>
    public static UpdateMovieDTO ToUpdateMovieDto(this Movie movie)
    {
        return new UpdateMovieDTO
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            Director = movie.Director,
            MovieGenre = movie.MovieGenre
        };
    }

    /// <summary>
    /// Converts a Movie domain model to a RemoveMovieDTO. This method takes a Movie object and maps its properties to a new RemoveMovieDTO object,
    /// which is used for removing movie information.
    /// </summary>
    /// <param name="movie">The Movie domain model to be converted.</param>
    /// <returns>A RemoveMovieDTO object containing the mapped properties from the Movie domain model.</returns>
    public static RemoveMovieDTO ToRemoveMovieDto(this Movie movie)
    {
        return new RemoveMovieDTO
        {
            Title = movie.Title
        };
    }

    /// <summary>
    /// Converts a CreateMovieDTO to a Movie domain model. This method takes a CreateMovieDTO object and maps its properties to a new Movie object,
    /// which is used for creating a new movie.
    /// </summary>
    /// <param name="dto">The CreateMovieDTO to be converted.</param>
    /// <returns>A Movie domain model containing the mapped properties from the CreateMovieDTO.</returns>
    public static Movie ToDomain(this CreateMovieDTO dto)
    {
        return new Movie
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            Director = dto.Director,
            MovieGenre = dto.MovieGenre
        };
    }

    /// <summary>
    /// Converts an UpdateMovieDTO to a Movie domain model. This method takes an UpdateMovieDTO object and maps its properties to a new Movie object,
    /// which is used for updating an existing movie.
    /// </summary>
    /// <param name="dto">The UpdateMovieDTO to be converted.</param>
    /// <returns>A Movie domain model containing the mapped properties from the UpdateMovieDTO.</returns>
    public static Movie ToDomain(this UpdateMovieDTO dto)
    {
        return new Movie
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            Director = dto.Director,
            MovieGenre = dto.MovieGenre
        };
    }
}