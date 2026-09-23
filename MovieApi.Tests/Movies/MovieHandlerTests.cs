using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.Create;
using MovieApi.Core.Features.Movies.Delete;
using MovieApi.Core.Features.Movies.GetById;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Features.Movies.Update;
using MovieApi.Domain.Entities;
using MovieApi.Tests.Fakes;

namespace MovieApi.Tests.Movies
{
    public sealed class MovieHandlerTests
    {
        [Fact]
        public async Task CreateMovie_WithExistingGenresAndActors_CreatesMovie()
        {
            var movies = new FakeMovieRepository();
            var genres = new FakeGenreRepository();
            var actors = new FakeActorRepository();
            var genre = CreateGenre("Sci-Fi");
            var actor = CreateActor("Keanu", "Reeves");
            genres.AddExisting(genre);
            actors.AddExisting(actor);
            var handler = new CreateMovieHandler(movies, genres, actors);
            var request = new CreateMovieRequest(
                " The Matrix ",
                " Sci-fi movie ",
                1999,
                136,
                [genre.Id],
                [actor.Id]);

            var result = await handler.HandleAsync(request, null, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value!.Id);
            Assert.Equal("The Matrix", result.Value.Title);
            Assert.Single(result.Value.Genres);
            Assert.Single(result.Value.Actors);
            Assert.Single(movies.Movies);
        }

        [Fact]
        public async Task CreateMovie_WithMissingActor_ReturnsValidationError()
        {
            var handler = new CreateMovieHandler(
                new FakeMovieRepository(),
                new FakeGenreRepository(),
                new FakeActorRepository());
            var request = new CreateMovieRequest(
                "The Matrix",
                null,
                1999,
                136,
                [],
                [Guid.NewGuid()]);

            var result = await handler.HandleAsync(request, null, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        }

        [Fact]
        public async Task CreateMovie_WithDuplicateActorIds_ReturnsValidationError()
        {
            var actorId = Guid.NewGuid();
            var handler = new CreateMovieHandler(
                new FakeMovieRepository(),
                new FakeGenreRepository(),
                new FakeActorRepository());
            var request = new CreateMovieRequest(
                "The Matrix",
                null,
                1999,
                136,
                [],
                [actorId, actorId]);

            var result = await handler.HandleAsync(request, null, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        }

        [Fact]
        public async Task UpdateMovie_ReplacesGenresAndActors()
        {
            var movies = new FakeMovieRepository();
            var genres = new FakeGenreRepository();
            var actors = new FakeActorRepository();
            var oldGenre = CreateGenre("Action");
            var newGenre = CreateGenre("Sci-Fi");
            var oldActor = CreateActor("Carrie-Anne", "Moss");
            var newActor = CreateActor("Keanu", "Reeves");
            genres.AddExisting(newGenre);
            actors.AddExisting(newActor);
            var movie = CreateMovieWithDetails(oldGenre, oldActor);
            await movies.AddAsync(movie, CancellationToken.None);
            var handler = new UpdateMovieHandler(movies, genres, actors);
            var request = new UpdateMovieRequest(
                "The Matrix Reloaded",
                "Updated",
                2003,
                138,
                [newGenre.Id],
                [newActor.Id]);

            var result = await handler.HandleAsync(movie.Id, request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("The Matrix Reloaded", result.Value!.Title);
            Assert.Single(result.Value.Genres);
            Assert.Equal(newGenre.Id, result.Value.Genres[0].Id);
            Assert.Single(result.Value.Actors);
            Assert.Equal(newActor.Id, result.Value.Actors[0].Id);
        }

        [Fact]
        public async Task GetMovies_ReturnsListItemsWithoutDetails()
        {
            var movies = new FakeMovieRepository();
            await movies.AddAsync(CreateMovie("The Matrix", 1999), CancellationToken.None);
            var handler = new GetMoviesHandler(movies);
            var request = new GetMoviesRequest(null, null, null, "title", "asc", 1, 10);

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.Items);
            Assert.Equal("The Matrix", result.Value.Items[0].Title);
            Assert.Equal(1999, result.Value.Items[0].ReleaseYear);
        }

        [Fact]
        public async Task GetMovies_AppliesSearchAndPagination()
        {
            var movies = new FakeMovieRepository();
            await movies.AddAsync(CreateMovie("The Matrix", 1999), CancellationToken.None);
            await movies.AddAsync(CreateMovie("John Wick", 2014), CancellationToken.None);
            var handler = new GetMoviesHandler(movies);
            var request = new GetMoviesRequest("matrix", null, null, "title", "asc", 1, 10);

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.Items);
            Assert.Equal("The Matrix", result.Value.Items[0].Title);
            Assert.Equal(1, result.Value.TotalCount);
        }

        [Fact]
        public async Task GetMovieById_WhenMovieExists_ReturnsDetails()
        {
            var genre = CreateGenre("Sci-Fi");
            var actor = CreateActor("Keanu", "Reeves");
            var movie = CreateMovieWithDetails(genre, actor);
            movie.Reviews.Add(CreateReview(movie.Id, "Great movie", 10));
            var movies = new FakeMovieRepository();
            await movies.AddAsync(movie, CancellationToken.None);
            var handler = new GetMovieByIdHandler(movies);

            var result = await handler.HandleAsync(movie.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(movie.Id, result.Value!.Id);
            Assert.Single(result.Value.Genres);
            Assert.Single(result.Value.Actors);
            Assert.Single(result.Value.Reviews);
        }

        [Fact]
        public async Task GetMovieById_WhenMovieDoesNotExist_ReturnsNotFound()
        {
            var handler = new GetMovieByIdHandler(new FakeMovieRepository());

            var result = await handler.HandleAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task DeleteMovie_WhenMovieExists_RemovesMovie()
        {
            var movies = new FakeMovieRepository();
            var movie = CreateMovie("Movie to delete", 2020);
            await movies.AddAsync(movie, CancellationToken.None);
            var handler = new DeleteMovieHandler(movies);

            var result = await handler.HandleAsync(movie.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(movies.Movies);
        }

        private static Movie CreateMovie(string title, int releaseYear)
        {
            var now = DateTime.UtcNow;

            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = null,
                ReleaseYear = releaseYear,
                DurationMinutes = 100,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static Movie CreateMovieWithDetails(Genre genre, Actor actor)
        {
            var movie = CreateMovie("The Matrix", 1999);
            movie.Description = "A hacker discovers the truth about his world.";

            movie.MovieGenres.Add(new MovieGenre
            {
                MovieId = movie.Id,
                Movie = movie,
                GenreId = genre.Id,
                Genre = genre
            });

            movie.MovieActors.Add(new MovieActor
            {
                MovieId = movie.Id,
                Movie = movie,
                ActorId = actor.Id,
                Actor = actor
            });

            return movie;
        }

        private static Genre CreateGenre(string name)
        {
            var now = DateTime.UtcNow;

            return new Genre
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static Actor CreateActor(string firstName, string lastName)
        {
            var now = DateTime.UtcNow;

            return new Actor
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static Review CreateReview(Guid movieId, string text, int rating)
        {
            var now = DateTime.UtcNow;

            return new Review
            {
                Id = Guid.NewGuid(),
                MovieId = movieId,
                AuthorId = Guid.NewGuid(),
                Text = text,
                Rating = rating,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }
}
