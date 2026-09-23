using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.GetPosterAccess
{
    public sealed class GetMoviePosterAccessHandler
    {
        private static readonly TimeSpan AccessLifetime =
            TimeSpan.FromMinutes(5);

        private readonly IMovieRepository _movieRepository;
        private readonly IObjectStorage _objectStorage;

        public GetMoviePosterAccessHandler(
            IMovieRepository movieRepository,
            IObjectStorage objectStorage)
        {
            _movieRepository = movieRepository;
            _objectStorage = objectStorage;
        }

        public async Task<OperationResult<MoviePosterAccessResponse>> HandleAsync(
            Guid movieId,
            CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetWithMediaAsync(
                movieId,
                cancellationToken);

            if (movie is null || movie.Poster is null)
            {
                return OperationResult<MoviePosterAccessResponse>.Failure(
                    OperationErrorType.NotFound,
                    "Movie poster was not found.");
            }

            try
            {
                var access = await _objectStorage.CreateReadUrlAsync(
                    movie.Poster.ObjectKey,
                    AccessLifetime,
                    cancellationToken);

                if (access is null)
                {
                    return OperationResult<MoviePosterAccessResponse>.Failure(
                        OperationErrorType.NotFound,
                        "Movie poster file was not found.");
                }

                return OperationResult<MoviePosterAccessResponse>.Success(
                    new MoviePosterAccessResponse(
                        access.Url,
                        access.ExpiresAtUtc));
            }
            catch (ObjectStorageUnavailableException)
            {
                return OperationResult<MoviePosterAccessResponse>.Failure(
                    OperationErrorType.Unavailable,
                    "Poster storage is temporarily unavailable.");
            }
        }
    }
}
