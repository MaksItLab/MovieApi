using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.UploadVideo
{
    public sealed record MovieVideoInfo(
        string ContentType,
        long SizeBytes,
        DateTime UploadedAt);

    public sealed record UploadMovieVideoCommand(
        Guid MovieId,
        UploadedFile File);

    public sealed class UploadMovieVideoHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;

        public UploadMovieVideoHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
        }

        public async Task<OperationResult<MovieVideoInfo>> HandleAsync(
            UploadMovieVideoCommand command,
            CancellationToken cancellationToken)
        {
            var validation = await VideoFileValidator.ValidateAsync(command.File, cancellationToken);
            if (!validation.IsSuccess)
            {
                return OperationResult<MovieVideoInfo>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var movie = await _movieRepository.GetWithMediaAsync(
                command.MovieId,
                cancellationToken);

            if (movie is null)
            {
                return OperationResult<MovieVideoInfo>.Failure(
                    OperationErrorType.NotFound,
                    "Movie was not found.");
            }

            if (movie.Video is not null)
            {
                return OperationResult<MovieVideoInfo>.Failure(
                    OperationErrorType.Conflict,
                    "Movie already has a video.");
            }

            var verifiedVideo = validation.Value!;
            var objectKey = $"videos/{movie.Id:N}/{Guid.NewGuid():N}{verifiedVideo.Extension}";

            try
            {
                await using var content = command.File.OpenReadStream();
                await _objectStorage.PutAsync(
                    new ObjectToStore(
                        objectKey,
                        verifiedVideo.ContentType,
                        command.File.Length,
                        content),
                    cancellationToken);
            }
            catch (ObjectStorageUnavailableException)
            {
                return OperationResult<MovieVideoInfo>.Failure(
                    OperationErrorType.Unavailable,
                    "Video storage is temporarily unavailable.");
            }

            var uploadedAt = DateTime.UtcNow;
            movie.Video = new MovieVideo
            {
                MovieId = movie.Id,
                ObjectKey = objectKey,
                OriginalFileName = command.File.OriginalFileName,
                ContentType = verifiedVideo.ContentType,
                SizeBytes = command.File.Length,
                UploadedAt = uploadedAt
            };

            try
            {
                await _movieRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                try
                {
                    await _objectStorage.DeleteIfExistsAsync(objectKey, cancellationToken);
                }
                catch
                {
                    // Ошибка сохранения PostgreSQL не должна быть заменена ошибкой cleanup.
                }

                throw;
            }

            return OperationResult<MovieVideoInfo>.Success(
                new MovieVideoInfo(
                    verifiedVideo.ContentType,
                    command.File.Length,
                    uploadedAt));
        }
    }
}
