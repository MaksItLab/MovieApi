using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Reviews.GetList
{
    public sealed class GetMovieReviewsHandler
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IReviewRepository _reviewRepository;

        public GetMovieReviewsHandler(
            IMovieRepository movieRepository,
            IReviewRepository reviewRepository)
        {
            _movieRepository = movieRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<OperationResult<IReadOnlyList<ReviewResponse>>> HandleAsync(
            Guid movieId,
            CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId, cancellationToken);
            if (movie is null)
            {
                return OperationResult<IReadOnlyList<ReviewResponse>>.Failure(
                    OperationErrorType.NotFound,
                    $"Movie with id='{movieId}' was not found.");
            }

            var reviews = await _reviewRepository.GetByMovieIdAsync(movieId, cancellationToken);

            return OperationResult<IReadOnlyList<ReviewResponse>>.Success(
                reviews.Select(ReviewMapper.ToResponse).ToList());
        }
    }
}
