using Microsoft.Extensions.Logging;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.UploadPoster;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.ReplacePoster
{
    public sealed record ReplaceMoviePosterCommand(Guid MovieId, UploadedFile File);

    public sealed class ReplaceMoviePosterHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<ReplaceMoviePosterHandler> _logger;

        public ReplaceMoviePosterHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage,
            ILogger<ReplaceMoviePosterHandler> logger)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
            _logger = logger;
        }

        public async Task<OperationResult<MoviePosterResponse>> HandleAsync(
            ReplaceMoviePosterCommand command,
            CancellationToken cancellationToken)
        {
            var validation = await PosterFileValidator.ValidateAsync(
                command.File,
                cancellationToken);

            if (!validation.IsSuccess)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var movie = await _movieRepository.GetWithMediaAsync(
                command.MovieId,
                cancellationToken);

            if (movie is null)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    OperationErrorType.NotFound,
                    "Movie was not found.");
            }

            if (movie.Poster is null)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    OperationErrorType.NotFound,
                    "Movie poster was not found.");
            }

            var verifiedFile = validation.Value!;
            var oldObjectKey = movie.Poster.ObjectKey;
            var newObjectKey =
                $"posters/{movie.Id:N}/{Guid.NewGuid():N}{verifiedFile.Extension}";

            try
            {
                await using var content = command.File.OpenReadStream();

                await _objectStorage.PutAsync(
                    new ObjectToStore(
                        newObjectKey,
                        verifiedFile.ContentType,
                        command.File.Length,
                        content),
                    cancellationToken);
            }
            catch (ObjectStorageUnavailableException)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    OperationErrorType.Unavailable,
                    "Poster storage is temporarily unavailable.");
            }

            var uploadedAt = DateTime.UtcNow;

            movie.Poster.ObjectKey = newObjectKey;
            movie.Poster.OriginalFileName = command.File.OriginalFileName;
            movie.Poster.ContentType = verifiedFile.ContentType;
            movie.Poster.SizeBytes = command.File.Length;
            movie.Poster.UploadedAt = uploadedAt;

            try
            {
                await _movieRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await TryDeleteNewObjectAsync(newObjectKey, cancellationToken);
                throw;
            }

            await TryCleanupOldObjectAsync(
                movie.Id,
                oldObjectKey,
                cancellationToken);

            return OperationResult<MoviePosterResponse>.Success(
                new MoviePosterResponse(
                    verifiedFile.ContentType,
                    command.File.Length,
                    uploadedAt));
        }

        private async Task TryDeleteNewObjectAsync(
            string objectKey,
            CancellationToken cancellationToken)
        {
            try
            {
                await _objectStorage.DeleteIfExistsAsync(objectKey, cancellationToken);
            }
            catch (ObjectStorageUnavailableException exception)
            {
                _logger.LogWarning(
                    exception,
                    "New poster object {ObjectKey} could not be removed after a database failure.",
                    objectKey);
            }
        }

        private async Task TryCleanupOldObjectAsync(
            Guid movieId,
            string oldObjectKey,
            CancellationToken cancellationToken)
        {
            try
            {
                await _objectStorage.DeleteIfExistsAsync(
                    oldObjectKey,
                    cancellationToken);
            }
            catch (ObjectStorageUnavailableException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Poster cleanup was deferred for movie {MovieId} and object {ObjectKey}.",
                    movieId,
                    oldObjectKey);
            }
        }
    }
}
