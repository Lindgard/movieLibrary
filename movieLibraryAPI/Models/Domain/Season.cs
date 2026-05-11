namespace movieLibrary.Models.Domain;

public class Season
{
    public string SeasonName { get; set; } = string.Empty;
    public Dictionary<int, List<Episode>> Episodes { get; set; } = new();
}