using MovieApi.Core.Common;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Tests.Fakes
{
    internal sealed class FakeGenreRepository : IGenreRepository
    {
        private readonly List<Genre> _genres = [];
        private readonly HashSet<Guid> _genreIdsWithMovies = [];

        public IReadOnlyList<Genre> Genres => _genres;

        public void AddExisting(Genre genre)
        {
            _genres.Add(genre);
        }

        public void MarkAsUsedByMovie(Guid id)
        {
            _genreIdsWithMovies.Add(id);
        }

        public Task<PagedResult<Genre>> GetAllAsync(
            GenreListQuery query,
            CancellationToken cancellationToken)
        {
            var genresQuery = _genres.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLowerInvariant();
                genresQuery = genresQuery.Where(genre => genre.Name.ToLowerInvariant().Contains(search));
            }

            genresQuery = (query.SortBy, query.SortDirection) switch
            {
                (GenreSortBy.Name, MovieApi.Core.Features.Genres.GetList.SortDirection.Asc) => genresQuery.OrderBy(genre => genre.Name),
                (GenreSortBy.Name, MovieApi.Core.Features.Genres.GetList.SortDirection.Desc) => genresQuery.OrderByDescending(genre => genre.Name),
                (GenreSortBy.CreatedAt, MovieApi.Core.Features.Genres.GetList.SortDirection.Asc) => genresQuery.OrderBy(genre => genre.CreatedAt),
                _ => genresQuery.OrderByDescending(genre => genre.CreatedAt)
            };

            var totalCount = genresQuery.Count();

            var items = genresQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Genre>(
                items,
                query.Page,
                query.PageSize,
                totalCount));
        }

        public Task<IReadOnlyList<Genre>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            var genres = _genres
                .Where(genre => ids.Contains(genre.Id))
                .ToList();

            return Task.FromResult<IReadOnlyList<Genre>>(genres);
        }

        public Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_genres.FirstOrDefault(genre => genre.Id == id));
        }

        public Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_genreIdsWithMovies.Contains(id));
        }

        public Task AddAsync(Genre genre, CancellationToken cancellationToken)
        {
            _genres.Add(genre);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Genre genre, CancellationToken cancellationToken)
        {
            var index = _genres.FindIndex(existing => existing.Id == genre.Id);
            if (index >= 0)
            {
                _genres[index] = genre;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Genre genre, CancellationToken cancellationToken)
        {
            _genres.Remove(genre);
            return Task.CompletedTask;
        }
    }
}
