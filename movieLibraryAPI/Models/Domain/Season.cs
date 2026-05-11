namespace movieLibrary.Models.Domain;

public class Season
{
    public int SeasonNumber { get; set; }
    public Dictionary<int, List<Episode>> Episodes { get; set; } = new();
}