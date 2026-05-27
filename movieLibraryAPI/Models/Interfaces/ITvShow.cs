using movieLibraryAPI.Models.Domain;
namespace movieLibraryAPI.Models.Interfaces;

public interface ITvShow
{
    string Title { get; set; }
    int ReleaseYear { get; set; }
    string Description { get; set; }
    int TotalEpisodes { get; set; }
    Genres TvShowGenre { get; set; }
}