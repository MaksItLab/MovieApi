using Microsoft.Extensions.Logging;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MovieApi.Core.Features.Movies.Delete
{
    public sealed class DeleteMovieHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<DeleteMovieHandler> _logger;

        public DeleteMovieHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage,
            ILogger<DeleteMovieHandler> logger)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task<OperationResult> HandleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetWithMediaAsync(
                id,
                cancellationToken);

            if (movie is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    "Movie was not found.");
            }

            var posterObjectKey = movie.Poster?.ObjectKey;
            var videoObjectKey = movie.Video?.ObjectKey;

            await _movieRepository.DeleteAsync(movie, cancellationToken);
            await _movieRepository.SaveChangesAsync(cancellationToken);

            await TryDeleteMediaAsync(
                movie.Id,
                posterObjectKey,
                "poster",
                cancellationToken);

            await TryDeleteMediaAsync(
                movie.Id,
                videoObjectKey,
                "video",
                cancellationToken);

            return OperationResult.Success();
        }

        private async Task TryDeleteMediaAsync(
            Guid movieId,
            string? objectKey,
            string mediaType,
            CancellationToken cancellationToken)
        {
            if (objectKey is null)
            {
                return;
            }

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
                    "{MediaType} cleanup was deferred after deleting movie {MovieId} and object {ObjectKey}.",
                    mediaType,
                    movieId,
                    objectKey);
            }
        }
    }
}
