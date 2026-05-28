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

    public MovieLibraryDbContext(DbContextOptions<MovieLibraryDbContext> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "movielibrary.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(m => m.Movies)
            .WithMany(t => t.MovieAndTvShowLists)
            .UsingEntity(j => j.ToTable("MovieListMovies"));

        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(t => t.TvShows)
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
