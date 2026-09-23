using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Reviews.Update
{
    public sealed class UpdateReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public UpdateReviewHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<OperationResult<ReviewResponse>> HandleAsync(
            Guid id,
            UpdateReviewRequest request,
            CancellationToken cancellationToken)
        {
            var validation = ReviewValidation.Validate(request);
            if (!validation.IsSuccess)
            {
                return OperationResult<ReviewResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
            if (review is null)
            {
                return OperationResult<ReviewResponse>.Failure(
                    OperationErrorType.NotFound,
                    $"Review with id='{id}' was not found.");
            }

            review.Text = request.Text.Trim();
            review.Rating = request.Rating;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<ReviewResponse>.Success(ReviewMapper.ToResponse(review));
        }
    }
}
