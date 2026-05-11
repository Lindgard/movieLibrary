using movieLibrary.Models.Domain;
using movieLibrary.Models.DTOs;

namespace movieLibrary.Mappings;

public static class MovieMappings
{
    public static MovieDTO ToDto(this Movie movie)
    {
        return new MovieDTO
        {
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            Director = movie.Director,
            MovieGenre = movie.MovieGenre
        };
    }

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