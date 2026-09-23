using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IActorRepository
    {
        Task<IReadOnlyList<Actor>> GetAllAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<Actor>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

        Task<Actor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(Actor actor, CancellationToken cancellationToken);

        Task UpdateAsync(Actor actor, CancellationToken cancellationToken);

        Task DeleteAsync(Actor actor, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
