using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.Create;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;

namespace MovieApi.Core.Features.Movies.Update
{
    public sealed class UpdateMovieHandler
    {
        private readonly IMovieRepository _movies;
        private readonly IGenreRepository _genres;
        private readonly IActorRepository _actors;

        public UpdateMovieHandler(
            IMovieRepository movies,
            IGenreRepository genres,
            IActorRepository actors)
        {
            _movies = movies;
            _genres = genres;
            _actors = actors;
        }

        public async Task<OperationResult<MovieDetailsResponse>> HandleAsync(
            Guid id,
            UpdateMovieRequest request,
            CancellationToken token)
        {
            var validation = CreateMovieHandler.ValidateMovieFields(
                request.Title,
                request.ReleaseYear,
                request.DurationMinutes);

            if (!validation.IsSuccess)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var genreIds = request.GenreIds ?? [];
            var actorIds = request.ActorIds ?? [];

            if (genreIds.Distinct().Count() != genreIds.Count)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "GenreIds must not contain duplicates.");
            }

            if (actorIds.Distinct().Count() != actorIds.Count)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "ActorIds must not contain duplicates.");
            }

            var genres = await _genres.GetByIdsAsync(genreIds, token);
            if (genres.Count != genreIds.Count)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "One or more genres were not found.");
            }

            var actors = await _actors.GetByIdsAsync(actorIds, token);
            if (actors.Count != actorIds.Count)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.Validation,
                    "One or more actors were not found.");
            }

            var movie = await _movies.GetDetailsByIdAsync(id, token);
            if (movie is null)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                    OperationErrorType.NotFound,
                    $"Film with id='{id}' was not found.");
            }

            movie.Title = request.Title.Trim();
            movie.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            movie.ReleaseYear = request.ReleaseYear;
            movie.DurationMinutes = request.DurationMinutes;
            movie.UpdatedAt = DateTime.UtcNow;

            movie.MovieGenres.Clear();
            movie.MovieActors.Clear();

            foreach (var genreId in genreIds)
            {
                movie.MovieGenres.Add(new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId,
                    Genre = genres.First(genre => genre.Id == genreId)
                });
            }

            foreach (var actorId in actorIds)
            {
                movie.MovieActors.Add(new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId,
                    Actor = actors.First(actor => actor.Id == actorId)
                });
            }

            await _movies.UpdateAsync(movie, token);

            return OperationResult<MovieDetailsResponse>.Success(MovieMapper.ToResponse(movie));
        }
    }
}
