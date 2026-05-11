namespace movieLibrary.Models.Domain;

public class Episode
{
    public string EpisodeName { get; set; } = string.Empty;
    public int EpisodeNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }
}