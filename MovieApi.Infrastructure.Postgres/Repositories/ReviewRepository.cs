using Microsoft.EntityFrameworkCore;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Postgres.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Postgres.Repositories
{
    public sealed class ReviewRepository : IReviewRepository
    {
        private readonly MovieDbContext _dbContext;

        public ReviewRepository(MovieDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Review>> GetByMovieIdAsync(
            Guid movieId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Where(review => review.MovieId == movieId)
                .OrderByDescending(review => review.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Reviews
                .FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
        }

        public async Task AddAsync(Review review, CancellationToken cancellationToken)
        {
            await _dbContext.Reviews.AddAsync(review, cancellationToken);
        }

        public Task UpdateAsync(Review review, CancellationToken cancellationToken)
        {
            _dbContext.Reviews.Update(review);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Review review, CancellationToken cancellationToken)
        {
            _dbContext.Reviews.Remove(review);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
