using Microsoft.Extensions.Logging;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;

namespace MovieApi.Core.Features.Movies.Create
{
    public sealed class CreateMovieHandler
    {
        private readonly IMovieRepository _movies;
        private readonly IGenreRepository _genres;
        private readonly IActorRepository _actors;

        public CreateMovieHandler(
            IMovieRepository movies,
            IGenreRepository genres,
            IActorRepository actors)
        {
            _movies = movies;
            _genres = genres;
            _actors = actors;
        }

        public async Task<OperationResult<MovieDetailsResponse>> HandleAsync(
            CreateMovieRequest request,
            ILogger logger,
            CancellationToken token)
        {
            var validation = ValidateMovieFields(
                request.Title,
                request.ReleaseYear,
                request.DurationMinutes);

            if (!validation.IsSuccess)
            {
                logger.LogWarning("Ошибки валидации");

                return OperationResult<MovieDetailsResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var genreIds = request.GenreIds ?? [];
            var actorIds = request.ActorIds ?? [];

            if (HasDuplicates(genreIds))
            {
                logger.LogWarning("Встречены одинаковые Id жанров");

                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "GenreIds must not contain duplicates.");
            }

            if (HasDuplicates(actorIds))
            {
                logger.LogWarning("Встречены одинаковые Id жанров");

                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "ActorIds must not contain duplicates.");
            }

            var genres = await _genres.GetByIdsAsync(genreIds, token);
            if (genres.Count != genreIds.Count)
            {
                logger.LogWarning("Искомый источник не был найден");

                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "One or more genres were not found.");
            }

            var actors = await _actors.GetByIdsAsync(actorIds, token);
            if (actors.Count != actorIds.Count)
            {
                logger.LogWarning("Искомый источник не был найден");

                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "One or more actors were not found.");
            }

            var now = DateTime.UtcNow;
            var movieId = Guid.NewGuid();

            var movie = new Movie
            {
                Id = movieId,
                Title = request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),
                ReleaseYear = request.ReleaseYear,
                DurationMinutes = request.DurationMinutes,
                CreatedAt = now,
                UpdatedAt = now,
                MovieGenres = genreIds
                    .Select(genreId => new MovieGenre
                    {
                        MovieId = movieId,
                        GenreId = genreId,
                        Genre = genres.First(genre => genre.Id == genreId)
                    })
                    .ToList(),
                MovieActors = actorIds
                    .Select(actorId => new MovieActor
                    {
                        MovieId = movieId,
                        ActorId = actorId,
                        Actor = actors.First(actor => actor.Id == actorId)
                    })
                    .ToList()
            };

            await _movies.AddAsync(movie, token);

            return OperationResult<MovieDetailsResponse>.Success(MovieMapper.ToResponse(movie));
        }

        internal static OperationResult ValidateMovieFields(
            string title,
            int releaseYear,
            int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Title is required.");
            }

            if (title.Trim().Length > 200)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "Title must not be longer than 200 characters.");
            }

            if (releaseYear < 1888)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "ReleaseYear cannot be less than 1888.");
            }

            if (durationMinutes < 1)
            {
                return OperationResult.Failure(
                    OperationErrorType.Validation,
                    "DurationMinutes cannot be less than 1.");
            }

            return OperationResult.Success();
        }

        private static bool HasDuplicates(IReadOnlyList<Guid> ids)
        {
            return ids.Distinct().Count() != ids.Count;
        }
    }
}
