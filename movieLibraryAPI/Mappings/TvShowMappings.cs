using movieLibraryService.Models.Domain;
using movieLibraryService.Models.DTOs.TvShowDTOs;

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
            Seasons = tvShow.Seasons?.Select(s => s.ToDto()).ToList(),
            Id = tvShow.Id
        };
    }

    public static TvShow ToDomain(this CreateTvShowDTO dto)
    {
        var tvShow = new TvShow
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            TvShowGenre = dto.TvShowGenre,
            Creator = dto.Creator,
            TotalEpisodes = dto.TotalEpisodes,
            Id = dto.Id
        };

        if (dto.Seasons != null)
        {
            tvShow.Seasons = dto.Seasons.Select(s => s.ToDomain()).ToList();
            foreach (var season in tvShow.Seasons)
            {
                season.TvShowId = tvShow.Id;
            }
        }
        return tvShow;
    }

    public static TvShow ToDomain(this UpdateTvShowDTO dto)
    {
        var tvShow = new TvShow
        {
            Title = dto.Title,
            ReleaseYear = dto.ReleaseYear,
            Description = dto.Description,
            TvShowGenre = dto.TvShowGenre,
            Creator = dto.Creator,
            TotalEpisodes = dto.TotalEpisodes,
            Id = dto.Id
        };

        if (dto.Seasons != null)
        {
            tvShow.Seasons = dto.Seasons.Select(s => s.ToDomain()).ToList();
            foreach (var season in tvShow.Seasons)
            {
                season.TvShowId = tvShow.Id;
            }
        }
        return tvShow;
    }

    public static TvShowSeasonDTO ToDto(this Season season)
    {
        return new TvShowSeasonDTO
        {
            SeasonNumber = season.SeasonNumber,
            Episodes = season.Episodes
                .GroupBy(e => e.SeasonNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ToDto()).ToList()
                )
        };
    }

    public static Season ToDomain(this TvShowSeasonDTO dto)
    {
        return new Season
        {
            SeasonNumber = dto.SeasonNumber,
            Episodes = (dto.Episodes ?? new Dictionary<int, List<TvShowEpisodeDTO>>())
                .SelectMany(x => x.Value.Select(e => e.ToDomain()))
                .ToList()
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