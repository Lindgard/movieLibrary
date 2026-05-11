namespace movieLibrary.Models.DTOs;

public class TvShowSeasonDTO
{
    public string SeasonName { get; set; } = string.Empty;
    public Dictionary<int, List<TvShowEpisodeDTO>> Season { get; set; } = new Dictionary<int, List<TvShowEpisodeDTO>>();
}