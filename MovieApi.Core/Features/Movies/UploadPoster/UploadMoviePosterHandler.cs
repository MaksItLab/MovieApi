using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.UploadPoster
{
    public sealed class UploadMoviePosterHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;

        public UploadMoviePosterHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
        }

        public async Task<OperationResult<MoviePosterResponse>> HandleAsync(
            UploadMoviePosterCommand command,
            CancellationToken cancellationToken)
        {
            var validation = await PosterFileValidator.ValidateAsync(command.File, cancellationToken);
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

            if (movie.Poster is not null)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    OperationErrorType.Conflict,
                    "Movie already has a poster.");
            }

            var verifiedFile = validation.Value!;
            var objectKey = $"posters/{movie.Id:N}/{Guid.NewGuid():N}{verifiedFile.Extension}";

            try
            {
                await using var content = command.File.OpenReadStream();
                await _objectStorage.PutAsync(
                    new ObjectToStore(
                        objectKey,
                        verifiedFile.ContentType,
                        command.File.Length,
                        content),
                    cancellationToken);
            }
            catch (ObjectStorageUnavailableException ex)
            {
                return OperationResult<MoviePosterResponse>.Failure(
                    OperationErrorType.Unavailable,
                    $"Poster storage is temporarily unavailable. Message: {ex.Message}");
            }

            var uploadedAt = DateTime.UtcNow;
            movie.Poster = new MoviePoster
            {
                MovieId = movie.Id,
                ObjectKey = objectKey,
                OriginalFileName = command.File.OriginalFileName,
                ContentType = verifiedFile.ContentType,
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
                    // Первичная ошибка PostgreSQL важнее ошибки компенсации.
                }

                throw;
            }

            return OperationResult<MoviePosterResponse>.Success(
                new MoviePosterResponse(
                    verifiedFile.ContentType,
                    command.File.Length,
                    uploadedAt));
        }
    }

    public sealed record UploadMoviePosterCommand(Guid MovieId, UploadedFile File);
}
