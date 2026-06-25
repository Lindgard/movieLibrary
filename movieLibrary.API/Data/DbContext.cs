using Microsoft.EntityFrameworkCore;
using movieLibraryService.Models.Domain;
using movieLibraryService.Models;

namespace movieLibraryAPI.Data;

public class MovieLibraryDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<TvShow> TvShows => Set<TvShow>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<MovieAndTvShowList> MovieAndTvShowLists => Set<MovieAndTvShowList>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RecoveryToken> RecoveryTokens => Set<RecoveryToken>();

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

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(320)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(32)
            .IsRequired();

        modelBuilder.Entity<RecoveryToken>(entity =>
        {
            entity.HasKey(rt => rt.RecoveryTokenId);

            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RecoveryTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rt => rt.UserId);
            entity.Property(rt => rt.TokenHash).IsRequired();
            entity.Property(rt => rt.TokenSalt).IsRequired();
        });
    }
}
