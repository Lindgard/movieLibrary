namespace movieLibrary.Models.DTOs;

public class TvShowSeasonDTO
{
    public int SeasonNumber { get; set; }
    public Dictionary<int, List<TvShowEpisodeDTO>> Season { get; set; } = new Dictionary<int, List<TvShowEpisodeDTO>>();
}