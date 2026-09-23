using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Reviews;
using MovieApi.Core.Security;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Reviews.Create
{
    public sealed class CreateReviewHandler
    {
        private const string CurrentUserUnavailableMessage =
            "Ќе удалось определить текущего пользовател€.";

        private readonly IMovieRepository _movieRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUser _currentUser;

        public CreateReviewHandler(
            IMovieRepository movieRepository,
            IReviewRepository reviewRepository,
            ICurrentUser currentUser)
        {
            _movieRepository = movieRepository;
            _reviewRepository = reviewRepository;
            _currentUser = currentUser;
        }

        public async Task<OperationResult<ReviewResponse>> HandleAsync(
            Guid movieId,
            CreateReviewRequest request,
            CancellationToken cancellationToken)
        {
            var validation = ReviewValidation.Validate(request);
            if (!validation.IsSuccess)
            {
                return OperationResult<ReviewResponse>.Failure(
                    validation.ErrorType!.Value,
                    validation.ErrorMessage!);
            }

            var movie = await _movieRepository.GetByIdAsync(movieId, cancellationToken);
            if (movie is null)
            {
                return OperationResult<ReviewResponse>.Failure(
                    OperationErrorType.NotFound,
                    $"Movie with id='{movieId}' was not found.");
            }

            var authorId = _currentUser.UserId;
            if (authorId is null)
            {
                return OperationResult<ReviewResponse>.Failure(
                    OperationErrorType.Unauthorized,
                    CurrentUserUnavailableMessage);
            }

            var now = DateTime.UtcNow;

            var review = new Review
            {
                Id = Guid.NewGuid(),
                MovieId = movieId,
                AuthorId = authorId.Value,
                Text = request.Text.Trim(),
                Rating = request.Rating,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);

            return OperationResult<ReviewResponse>.Success(ReviewMapper.ToResponse(review));
        }
    }
}
