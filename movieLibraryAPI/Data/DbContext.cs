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

    /// <summary>
    /// Configures the many-to-many relationships between MovieAndTvShowList and Movie, and between MovieAndTvShowList and TvShow.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(l => l.Movies)
            .WithMany(t => t.MovieAndTvShowLists)
            .UsingEntity<Dictionary<string, object>>(
                "MovieListMovies",
                right => right.HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey("MovieId")
                    .OnDelete(DeleteBehavior.Cascade),
                Left => Left.HasOne<MovieAndTvShowList>()
                    .WithMany()
                    .HasForeignKey("MovieAndTvShowListId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("MovieAndTvShowListId", "MovieId");
                }
            );

        modelBuilder.Entity<MovieAndTvShowList>()
            .HasMany(l => l.Shows)
            .WithMany(s => s.MovieAndTvShowLists)
            .UsingEntity<Dictionary<string, object>>(
                "TvShowListShows",
                right => right.HasOne<TvShow>()
                    .WithMany()
                    .HasForeignKey("TvShowId")
                    .OnDelete(DeleteBehavior.Cascade),
                Left => Left.HasOne<MovieAndTvShowList>()
                    .WithMany()
                    .HasForeignKey("MovieAndTvShowListId")
                    .OnDelete(DeleteBehavior.Cascade),
                Join =>
                {
                    Join.HasKey("MovieAndTvShowListId", "TvShowId");
                }
            );

        modelBuilder.Entity<Season>()
            .HasMany(s => s.Episodes)
            .WithOne(e => e.Season)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TvShow>()
            .HasMany(t => t.Seasons)
            .WithOne(s => s.TvShow)
            .HasForeignKey(s => s.TvShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
