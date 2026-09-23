using Microsoft.EntityFrameworkCore;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Postgres.Persistence
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies => Set<Movie>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<Actor> Actors => Set<Actor>();

        public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();

        public DbSet<MovieActor> MovieActors => Set<MovieActor>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<User> Users => Set<User>();

        public DbSet<MoviePoster> MoviePosters => Set<MoviePoster>();

        public DbSet<MovieVideo> MovieVideos => Set<MovieVideo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(movie => movie.Id);

                entity.Property(movie => movie.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(movie => movie.Description)
                    .HasMaxLength(2000);

                entity.Property(movie => movie.ReleaseYear)
                    .IsRequired();

                entity.Property(movie => movie.DurationMinutes)
                    .IsRequired();

                entity.Property(movie => movie.CreatedAt)
                    .IsRequired();

                entity.Property(movie => movie.UpdatedAt)
                    .IsRequired();

                entity.HasMany(movie => movie.MovieGenres)
                    .WithOne(movieGenres => movieGenres.Movie)
                    .HasForeignKey(movieGenres => movieGenres.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(movie => movie.MovieActors)
                    .WithOne(movieActors => movieActors.Movie)
                    .HasForeignKey(movieActors => movieActors.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(movie => movie.Reviews)
                    .WithOne(review => review.Movie)
                    .HasForeignKey(review => review.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(genre => genre.Id);

                entity.Property(genre => genre.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(genre => genre.CreatedAt)
                    .IsRequired();

                entity.Property(genre => genre.UpdatedAt)
                    .IsRequired();

                entity.HasMany(genre => genre.MovieGenres)
                    .WithOne(movieGenres => movieGenres.Genre)
                    .HasForeignKey(movieGenres => movieGenres.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Actor>(entity =>
            {

                entity.HasKey(actor => actor.Id);

                entity.Property(actor => actor.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(actor => actor.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(actor => actor.BirthDate);

                entity.Property(actor => actor.CreatedAt)
                    .IsRequired();

                entity.Property(actor => actor.UpdatedAt)
                    .IsRequired();

                entity.HasMany(actor => actor.MovieActors)
                    .WithOne(movieActors => movieActors.Actor)
                    .HasForeignKey(movieActors => movieActors.ActorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MovieGenre>(entity =>
            {
                entity.HasKey(movieGenre => new
                {
                    movieGenre.MovieId,
                    movieGenre.GenreId
                });

                entity.HasOne(movieGenre => movieGenre.Movie)
                    .WithMany(movie => movie.MovieGenres)
                    .HasForeignKey(movieGenre => movieGenre.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(movieGenre => movieGenre.Genre)
                    .WithMany(genre => genre.MovieGenres)
                    .HasForeignKey(movieGenre => movieGenre.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MovieActor>(entity =>
            {
                entity.HasKey(movieActor => new
                {
                    movieActor.MovieId,
                    movieActor.ActorId
                });

                entity.HasOne(movieActor => movieActor.Movie)
                    .WithMany(movie => movie.MovieActors)
                    .HasForeignKey(movieActor => movieActor.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(movieActor => movieActor.Actor)
                    .WithMany(actor => actor.MovieActors)
                    .HasForeignKey(movieActor => movieActor.ActorId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(review => review.Id);

                entity.Property(review => review.MovieId)
                    .IsRequired();

                entity.Property(review => review.AuthorId)
                    .IsRequired();

                entity.Property(review => review.Text)
                    .HasMaxLength(2000)
                    .IsRequired();

                entity.Property(review => review.Rating)
                    .IsRequired();

                entity.Property(review => review.CreatedAt)
                    .IsRequired();

                entity.Property(review => review.UpdatedAt)
                    .IsRequired();

                entity.HasOne(review => review.Movie)
                    .WithMany(movie => movie.Reviews)
                    .HasForeignKey(review => review.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(review => review.Author)
                    .WithMany()
                    .HasForeignKey(review => review.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(user => user.Id);

                entity.Property(user => user.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(user => user.NormalizedEmail)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(user => user.PasswordHash)
                    .IsRequired();

                entity.Property(user => user.CreatedAt)
                    .IsRequired();

                entity.Property(user => user.UpdatedAt)
                    .IsRequired();

                entity.Property(user => user.Role)
                    .HasConversion<string>()
                    .HasMaxLength(32)
                    .HasDefaultValue(UserRole.User)
                    .IsRequired();

                entity.HasIndex(user => user.NormalizedEmail)
                    .IsUnique();
            });

            modelBuilder.Entity<MoviePoster>(entity =>
            {
                entity.ToTable("MoviePosters");
                entity.HasKey(poster => poster.MovieId);

                entity.Property(poster => poster.ObjectKey)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(poster => poster.OriginalFileName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(poster => poster.ContentType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(poster => poster.SizeBytes).IsRequired();
                entity.Property(poster => poster.UploadedAt).IsRequired();

                entity.HasOne(poster => poster.Movie)
                    .WithOne(movie => movie.Poster)
                    .HasForeignKey<MoviePoster>(poster => poster.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<MovieVideo>(entity =>
            {
                entity.ToTable("MovieVideos");
                entity.HasKey(video => video.MovieId);

                entity.Property(video => video.ObjectKey)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(video => video.OriginalFileName)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(video => video.ContentType)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(video => video.SizeBytes).IsRequired();
                entity.Property(video => video.UploadedAt).IsRequired();

                entity.HasOne(video => video.Movie)
                    .WithOne(movie => movie.Video)
                    .HasForeignKey<MovieVideo>(video => video.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
