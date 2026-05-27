namespace movieLibraryAPI.Models.DTOs;

public class TvShowSeasonDTO
{
    public int SeasonNumber { get; set; }
    public Dictionary<int, List<TvShowEpisodeDTO>> Episodes { get; set; } = new Dictionary<int, List<TvShowEpisodeDTO>>();
}