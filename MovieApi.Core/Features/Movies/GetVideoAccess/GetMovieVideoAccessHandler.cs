using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.GetVideoAccess
{
    public sealed class GetMovieVideoAccessHandler
    {
        private static readonly TimeSpan AccessLifetime =
            TimeSpan.FromMinutes(10);

        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;

        public GetMovieVideoAccessHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
        }

        public async Task<OperationResult<MovieVideoAccessResponse>> HandleAsync(
            Guid movieId,
            CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetWithMediaAsync(
                movieId,
                cancellationToken);

            if (movie is null || movie.Video is null)
            {
                return OperationResult<MovieVideoAccessResponse>.Failure(
                    OperationErrorType.NotFound,
                    "Movie video was not found.");
            }

            try
            {
                var access = await _objectStorage.CreateReadUrlAsync(
                    movie.Video.ObjectKey,
                    AccessLifetime,
                    cancellationToken);

                if (access is null)
                {
                    return OperationResult<MovieVideoAccessResponse>.Failure(
                        OperationErrorType.NotFound,
                        "Movie video file was not found.");
                }

                return OperationResult<MovieVideoAccessResponse>.Success(
                    new MovieVideoAccessResponse(
                        access.Url,
                        access.ExpiresAtUtc));
            }
            catch (ObjectStorageUnavailableException)
            {
                return OperationResult<MovieVideoAccessResponse>.Failure(
                    OperationErrorType.Unavailable,
                    "Video storage is temporarily unavailable.");
            }
        }
    }
}
