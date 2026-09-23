using MovieApi.Contracts.Reviews;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Reviews.Create;
using MovieApi.Core.Features.Reviews.Delete;
using MovieApi.Core.Features.Reviews.GetList;
using MovieApi.Core.Features.Reviews.Update;
using MovieApi.Domain.Entities;
using MovieApi.Tests.Fakes;

namespace MovieApi.Tests.Reviews
{
    public sealed class ReviewHandlerTests
    {
        [Fact]
        public async Task CreateReview_WhenMovieExists_ReturnsReview()
        {
            var movies = new FakeMovieRepository();
            var reviews = new FakeReviewRepository();
            var movie = CreateMovie();
            await movies.AddAsync(movie, CancellationToken.None);
            var currentUser = new FakeCurrentUser();
            var handler = new CreateReviewHandler(movies, reviews, currentUser);
            var request = new CreateReviewRequest("Great movie", 9);

            var result = await handler.HandleAsync(movie.Id, request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(movie.Id, result.Value!.MovieId);
            Assert.Equal("Great movie", result.Value.Text);
            Assert.Equal(9, result.Value.Rating);
        }

        [Fact]
        public async Task CreateReview_WhenMovieDoesNotExist_ReturnsNotFound()
        {
            var currentUser = new FakeCurrentUser();

            var handler = new CreateReviewHandler(
                new FakeMovieRepository(),
                new FakeReviewRepository(),
                currentUser);
            var request = new CreateReviewRequest("Great movie", 9);

            var result = await handler.HandleAsync(Guid.NewGuid(), request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task CreateReview_WithEmptyText_ReturnsValidationError()
        {
            var movies = new FakeMovieRepository();
            var reviews = new FakeReviewRepository();
            var movie = CreateMovie();
            await movies.AddAsync(movie, CancellationToken.None);
            var currentUser = new FakeCurrentUser();
            var handler = new CreateReviewHandler(movies, reviews, currentUser);
            var request = new CreateReviewRequest(" ", 9);

            var result = await handler.HandleAsync(movie.Id, request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public async Task CreateReview_WhenRatingIsOutOfRange_ReturnsValidationError(int rating)
        {
            var movies = new FakeMovieRepository();
            var reviews = new FakeReviewRepository();
            var movie = CreateMovie();
            await movies.AddAsync(movie, CancellationToken.None);
            var currentUser = new FakeCurrentUser();
            var handler = new CreateReviewHandler(movies, reviews, currentUser);
            var request = new CreateReviewRequest("Great movie", rating);

            var result = await handler.HandleAsync(movie.Id, request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        }

        [Fact]
        public async Task GetMovieReviews_WhenMovieExists_ReturnsReviews()
        {
            var movies = new FakeMovieRepository();
            var reviews = new FakeReviewRepository();
            var movie = CreateMovie();
            await movies.AddAsync(movie, CancellationToken.None);
            await reviews.AddAsync(CreateReview(movie.Id), CancellationToken.None);
            var handler = new GetMovieReviewsHandler(movies, reviews);

            var result = await handler.HandleAsync(movie.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal(movie.Id, result.Value![0].MovieId);
        }

        [Fact]
        public async Task UpdateReview_WhenReviewExists_ReturnsUpdatedReview()
        {
            var reviews = new FakeReviewRepository();
            var review = CreateReview(Guid.NewGuid());
            await reviews.AddAsync(review, CancellationToken.None);
            var handler = new UpdateReviewHandler(reviews);
            var request = new UpdateReviewRequest("Updated text", 7);

            var result = await handler.HandleAsync(review.Id, request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Updated text", result.Value.Text);
            Assert.Equal(7, result.Value.Rating);
        }

        [Fact]
        public async Task DeleteReview_WhenReviewExists_RemovesReview()
        {
            var reviews = new FakeReviewRepository();
            var review = CreateReview(Guid.NewGuid());
            await reviews.AddAsync(review, CancellationToken.None);
            var handler = new DeleteReviewHandler(reviews);

            var result = await handler.HandleAsync(review.Id, CancellationToken.None);
            var deleted = await reviews.GetByIdAsync(review.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Null(deleted);
        }

        private static Movie CreateMovie()
        {
            var now = DateTime.UtcNow;

            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = "The Matrix",
                Description = null,
                ReleaseYear = 1999,
                DurationMinutes = 136,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static Review CreateReview(Guid movieId)
        {
            var now = DateTime.UtcNow;

            return new Review
            {
                Id = Guid.NewGuid(),
                MovieId = movieId,
                AuthorId = Guid.NewGuid(),
                Text = "Great movie",
                Rating = 9,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }
}
