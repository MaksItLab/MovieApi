using MovieApi.Core.Common;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IGenreRepository
    {
        Task<PagedResult<Genre>> GetAllAsync(GenreListQuery query, CancellationToken cancellationToken);
        Task<IReadOnlyList<Genre>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
        Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Genre genre, CancellationToken cancellationToken);
        Task UpdateAsync(Genre genre, CancellationToken cancellationToken);
        Task DeleteAsync(Genre genre, CancellationToken cancellationToken);
    }
}
