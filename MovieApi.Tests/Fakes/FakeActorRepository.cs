using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Tests.Fakes
{
    internal sealed class FakeActorRepository : IActorRepository
    {
        private readonly List<Actor> _actors = [];
        private readonly HashSet<Guid> _actorIdsWithMovies = [];

        public IReadOnlyList<Actor> Actors => _actors;

        public void AddExisting(Actor actor)
        {
            _actors.Add(actor);
        }

        public void MarkAsUsedByMovie(Guid id)
        {
            _actorIdsWithMovies.Add(id);
        }

        public Task<IReadOnlyList<Actor>> GetAllAsync(CancellationToken cancellationToken)
        {
            var actors = _actors
                .OrderBy(actor => actor.LastName)
                .ThenBy(actor => actor.FirstName)
                .ToList();

            return Task.FromResult<IReadOnlyList<Actor>>(actors);
        }

        public Task<IReadOnlyList<Actor>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            var actors = _actors
                .Where(actor => ids.Contains(actor.Id))
                .ToList();

            return Task.FromResult<IReadOnlyList<Actor>>(actors);
        }

        public Task<Actor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_actors.FirstOrDefault(actor => actor.Id == id));
        }

        public Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_actorIdsWithMovies.Contains(id));
        }

        public Task AddAsync(Actor actor, CancellationToken cancellationToken)
        {
            _actors.Add(actor);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Actor actor, CancellationToken cancellationToken)
        {
            var index = _actors.FindIndex(existing => existing.Id == actor.Id);
            if (index >= 0)
            {
                _actors[index] = actor;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Actor actor, CancellationToken cancellationToken)
        {
            _actors.Remove(actor);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
