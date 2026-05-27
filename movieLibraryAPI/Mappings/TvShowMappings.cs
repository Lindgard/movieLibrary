using movieLibraryAPI.Models.Domain;
using movieLibraryAPI.Models.DTOs;

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
            Creator = tvShow.Creator,
            TotalEpisodes = tvShow.TotalEpisodes,
            Season = tvShow.Season?.ToDto(),
            Id = tvShow.Id
        };
    }

    public static TvShow ToDomain(this CreateTvShowDTO dto)
    {
        return new TvShow
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            TvShowGenre = dto.TvShowGenre,
            Creator = dto.Creator,
            TotalEpisodes = dto.TotalEpisodes,
            Season = dto.Season?.ToDomain(),
            Id = dto.Id
        };
    }

    public static TvShow ToDomain(this UpdateTvShowDTO dto)
    {
        return new TvShow
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            TvShowGenre = dto.TvShowGenre,
            Creator = dto.Creator,
            TotalEpisodes = dto.TotalEpisodes,
            Season = dto.Season?.ToDomain(),
            Id = dto.Id
        };
    }

    public static TvShowSeasonDTO ToDto(this Season season)
    {
        return new TvShowSeasonDTO
        {
            SeasonNumber = season.SeasonNumber,
            Episodes = season.Episodes.ToDictionary(
                x => x.Key,
                x => x.Value.Select(e => e.ToDto()).ToList())
        };
    }

    public static Season ToDomain(this TvShowSeasonDTO dto)
    {
        return new Season
        {
            SeasonNumber = dto.SeasonNumber,
            Episodes = dto.Episodes.ToDictionary(
                x => x.Key,
                x => x.Value.Select(e => e.ToDomain()).ToList())
        };
    }

    public static TvShowEpisodeDTO ToDto(this Episode episode)
    {
        return new TvShowEpisodeDTO
        {
            EpisodeName = episode.EpisodeName,
            EpisodeNumber = episode.EpisodeNumber,
            Description = episode.Description,
            SeasonNumber = episode.SeasonNumber
        };
    }

    public static Episode ToDomain(this TvShowEpisodeDTO dto)
    {
        return new Episode
        {
            EpisodeName = dto.EpisodeName,
            EpisodeNumber = dto.EpisodeNumber,
            Description = dto.Description,
            SeasonNumber = dto.SeasonNumber
        };
    }
}