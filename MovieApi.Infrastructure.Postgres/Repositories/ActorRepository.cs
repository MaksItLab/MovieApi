using Microsoft.EntityFrameworkCore;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Postgres.Persistence;

namespace MovieApi.Infrastructure.Postgres.Repositories
{
    public sealed class ActorRepository : IActorRepository
    {
        private readonly MovieDbContext _dbContext;

        public ActorRepository(MovieDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Actor>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Actors
                .AsNoTracking()
                .OrderBy(actor => actor.LastName)
                .ThenBy(actor => actor.FirstName)
                .ToListAsync(cancellationToken);
        }

        public async Task<Actor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Actors
                .FirstOrDefaultAsync(actor => actor.Id == id, cancellationToken);
        }

        public async Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.MovieActors
                .AnyAsync(movieActor => movieActor.ActorId == id, cancellationToken);
        }

        public async Task AddAsync(Actor actor, CancellationToken cancellationToken)
        {
            await _dbContext.Actors.AddAsync(actor, cancellationToken);
        }

        public Task UpdateAsync(Actor actor, CancellationToken cancellationToken)
        {
            _dbContext.Actors.Update(actor);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Actor actor, CancellationToken cancellationToken)
        {
            _dbContext.Actors.Remove(actor);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Actor>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
        {
            if (ids is null || ids.Count == 0)
            {
                return [];
            }

            return await _dbContext.Actors.Where(actor => ids.Contains(actor.Id)).ToListAsync(cancellationToken);
        }
    }
}
