namespace movieLibraryAPI.Models.DTOs.TvShowDTOs;

public class TvShowEpisodeDTO
{
    public string EpisodeName { get; set; } = string.Empty;
    public int EpisodeNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }
}