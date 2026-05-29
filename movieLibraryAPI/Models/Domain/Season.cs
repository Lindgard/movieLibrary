namespace movieLibraryAPI.Models.Domain;

public class Season
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int SeasonNumber { get; set; }
    public Guid TvShowId { get; set; }
    public TvShow TvShow { get; set; } = null!;

    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}