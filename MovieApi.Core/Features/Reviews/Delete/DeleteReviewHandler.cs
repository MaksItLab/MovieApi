using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Reviews.Delete
{
    public sealed class DeleteReviewHandler
    {
        private readonly IReviewRepository _reviewRepository;

        public DeleteReviewHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<OperationResult> HandleAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
            if (review is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    $"Review with id='{id}' was not found.");
            }

            await _reviewRepository.DeleteAsync(review, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
    }
}
