using movieLibrary.Models.DTOs;
using movieLibrary.Models.Domain;
using movieLibrary.Models.TvShows;

namespace movieLibraryAPI.Mappings;

public static class TvShowMappings
{
    public static TvShowDTO ToDto(this TvShow tvShow)
    {
        return new TvShowDTO
        {
            Title = tvShow.Title,
            ReleaseYear = tvShow.ReleaseYear,
            Description = tvShow.Description,
            TvShowGenre = tvShow.TvShowGenre,
            Creator = tvShow.Creator
        };
    }

    public static TvShow ToDomain(this CreateTvShowRequestDTO createTvShowRequestDTO)
    {
        return new TvShow
        {
            Title = createTvShowRequestDTO.Title,
            ReleaseYear = createTvShowRequestDTO.ReleaseYear,
            Description = createTvShowRequestDTO.Description,
            TvShowGenre = createTvShowRequestDTO.TvShowGenre,
            Season = createTvShowRequestDTO.Season,
            Creator = createTvShowRequestDTO.Creator
        };
    }

    public static TvShow ToDomain(this UpdateTvShowDTO updateTvShowDTO)
    {
        return new TvShow
        {
            Title = updateTvShowDTO.Title,
            ReleaseYear = updateTvShowDTO.ReleaseYear,
            Description = updateTvShowDTO.Description,
            TvShowGenre = updateTvShowDTO.TvShowGenre,
            Creator = updateTvShowDTO.Creator
        };
    }
}