using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<IReadOnlyList<Review>> GetByMovieIdAsync(
            Guid movieId,
            CancellationToken cancellationToken);

        Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(Review review, CancellationToken cancellationToken);

        Task UpdateAsync(Review review, CancellationToken cancellationToken);

        Task DeleteAsync(Review review, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
