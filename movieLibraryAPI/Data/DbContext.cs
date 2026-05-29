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
    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql($"Host=localhost;Database=movielibrary;Username=postgres;Password=postgres");
    }

    /// <summary>
    /// Initializes a new instance of the MovieLibraryDbContext class. 
    /// This constructor takes DbContextOptions as a parameter, which allows you to configure the context with specific options such as the database provider and connection string.
    /// The constructor also sets the DbPath property to the path of the local application data folder, where the SQLite database file will be stored. 
    /// This is useful for development and testing purposes, as it allows the database to be easily accessed and managed on the local machine.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public MovieLibraryDbContext(DbContextOptions<MovieLibraryDbContext> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "movielibrary.db");
    }

    /// <summary>
    /// Configures the entity relationships and mappings for the MovieLibraryDbContext. This method is called by the Entity Framework when the model is being created.
    /// It defines how the entities (Movie, TvShow, Season, Episode, MovieAndTvShowList) are related to each other and how they should be mapped to the database tables.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder used to configure the entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(m => m.Movies)
            .WithMany(t => t.MovieAndTvShowLists)
            .UsingEntity(j => j.ToTable("MovieListMovies"));

        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(t => t.Shows)
            .WithMany(l => l.MovieAndTvShowLists)
            .UsingEntity(j => j.ToTable("TvShowListTvShows"));

        modelBuilder.Entity<Season>()
            .HasMany(s => s.Episodes)
            .WithOne(e => e.Season)
            .HasForeignKey(e => e.SeasonId);

        modelBuilder.Entity<TvShow>()
            .HasMany(t => t.Seasons)
            .WithOne(s => s.TvShow)
            .HasForeignKey(s => s.TvShowId);
    }
}
