using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Reviews.GetById
{
    public sealed class GetReviewByIdHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewByIdHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<OperationResult<ReviewResponse>> HandleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
            if (review is null)
            {
                return OperationResult<ReviewResponse>.Failure(
                    OperationErrorType.NotFound,
                    $"Review with id='{id}' was not found.");
            }

            return OperationResult<ReviewResponse>.Success(ReviewMapper.ToResponse(review));
        }
    }
}
