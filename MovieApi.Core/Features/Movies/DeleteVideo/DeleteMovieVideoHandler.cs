using Microsoft.Extensions.Logging;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;

namespace MovieApi.Core.Features.Movies.DeleteVideo
{
    public sealed record DeleteMovieVideoCommand(Guid MovieId);

    public class DeleteMovieVideoHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<DeleteMovieVideoHandler> _logger;

        public DeleteMovieVideoHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage,
            ILogger<DeleteMovieVideoHandler> logger)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task<OperationResult> HandleAsync(
            DeleteMovieVideoCommand command,
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

            if (movie.Video is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    "Movie video was not found.");
            }

            var objectKey = movie.Video.ObjectKey;

            await _movieRepository.RemoveVideoAsync(
                movie.Video,
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
                    "Video cleanup was deferred for movie {MovieId} and object {ObjectKey}.",
                    movieId,
                    objectKey);
            }
        }
    }
}
