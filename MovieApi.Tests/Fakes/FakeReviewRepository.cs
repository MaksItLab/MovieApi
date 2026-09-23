using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Tests.Fakes
{
    internal sealed class FakeReviewRepository : IReviewRepository
    {
        private readonly List<Review> _reviews = [];

        public IReadOnlyList<Review> Reviews => _reviews;

        public Task<IReadOnlyList<Review>> GetByMovieIdAsync(
            Guid movieId,
            CancellationToken cancellationToken)
        {
            var reviews = _reviews
                .Where(review => review.MovieId == movieId)
                .OrderByDescending(review => review.CreatedAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<Review>>(reviews);
        }

        public Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_reviews.FirstOrDefault(review => review.Id == id));
        }

        public Task AddAsync(Review review, CancellationToken cancellationToken)
        {
            _reviews.Add(review);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Review review, CancellationToken cancellationToken)
        {
            var index = _reviews.FindIndex(existing => existing.Id == review.Id);
            if (index >= 0)
            {
                _reviews[index] = review;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Review review, CancellationToken cancellationToken)
        {
            _reviews.Remove(review);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
