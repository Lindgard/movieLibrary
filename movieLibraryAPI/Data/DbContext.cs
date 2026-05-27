using Microsoft.EntityFrameworkCore;
using movieLibraryAPI.Models.Domain;
using movieLibraryAPI.Models;

namespace movieLibraryAPI.Data;

public class MovieLibraryDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<TvShow> TvShows => Set<TvShow>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<MovieAndTvShowList> MovieAndTvShowLists => Set<MovieAndTvShowList>();

    public MovieLibraryDbContext(DbContextOptions<MovieLibraryDbContext> options) : base(options)
    {

    }
}
