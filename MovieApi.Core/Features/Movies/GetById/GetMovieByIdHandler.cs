using MovieApi.Contracts.Common;
using MovieApi.Contracts.Genres;
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

namespace MovieApi.Core.Features.Movies.GetById
{
    public sealed class GetMovieByIdHandler
    {
        private readonly IMovieRepository _movies;

        public GetMovieByIdHandler(IMovieRepository movies)
        {
            _movies = movies;
        }

        public async Task<OperationResult<MovieDetailsResponse>> HandleAsync(
            Guid id,
            CancellationToken token)
        {
            var movie = await _movies.GetDetailsByIdAsync(id, token);

            if (movie is null)
            {
                return OperationResult<MovieDetailsResponse>.Failure(
                        OperationErrorType.NotFound,
                        "Film was not found.");
            }

            var response = MovieMapper.ToResponse(movie);

            return OperationResult<MovieDetailsResponse>.Success(response);
        }
    }
}
