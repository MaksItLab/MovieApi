using Microsoft.Extensions.Logging;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.DeletePoster
{
    public sealed record DeleteMoviePosterCommand(Guid MovieId);

    public sealed class DeleteMoviePosterHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<DeleteMoviePosterHandler> _logger;

        public DeleteMoviePosterHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage,
            ILogger<DeleteMoviePosterHandler> logger)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task<OperationResult> HandleAsync(
            DeleteMoviePosterCommand command,
            CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetWithMediaAsync(
                command.MovieId,
                cancellationToken);

            if (movie is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    "Movie was not found.");
            }

            if (movie.Poster is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    "Movie poster was not found.");
            }

            var objectKey = movie.Poster.ObjectKey;

            _movieRepository.DeleteMoviePoster(
                movie.Poster,
                cancellationToken);

            await _movieRepository.SaveChangesAsync(cancellationToken);

            await TryCleanupObjectAsync(
                movie.Id,
                objectKey,
                cancellationToken);

            return OperationResult.Success();
        }

        private async Task TryCleanupObjectAsync(
            Guid movieId,
            string objectKey,
            CancellationToken cancellationToken)
        {
            try
            {
                await _objectStorage.DeleteIfExistsAsync(
                    objectKey,
                    cancellationToken);
            }
            catch (ObjectStorageUnavailableException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Poster cleanup was deferred for movie {MovieId} and object {ObjectKey}.",
                    movieId,
                    objectKey);
            }
        }
    }
}
