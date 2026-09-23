using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Tests.Fakes
{
    internal sealed class FakeMovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = [];

        public IReadOnlyList<Movie> Movies => _movies;

        public Task<PagedResult<Movie>> GetAllAsync(
            MovieListQuery query,
            CancellationToken cancellationToken)
        {
            var moviesQuery = _movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLowerInvariant();
                moviesQuery = moviesQuery.Where(movie =>
                    movie.Title.ToLowerInvariant().Contains(search)
                    || (movie.Description != null
                        && movie.Description.ToLowerInvariant().Contains(search)));
            }

            if (query.FromYear.HasValue)
            {
                moviesQuery = moviesQuery.Where(movie => movie.ReleaseYear >= query.FromYear.Value);
            }

            if (query.ToYear.HasValue)
            {
                moviesQuery = moviesQuery.Where(movie => movie.ReleaseYear <= query.ToYear.Value);
            }

            moviesQuery = (query.SortBy, query.SortDirection) switch
            {
                (MovieSortBy.Title, SortDirection.Asc) => moviesQuery.OrderBy(movie => movie.Title),
                (MovieSortBy.Title, SortDirection.Desc) => moviesQuery.OrderByDescending(movie => movie.Title),
                (MovieSortBy.ReleaseYear, SortDirection.Asc) => moviesQuery.OrderBy(movie => movie.ReleaseYear),
                (MovieSortBy.ReleaseYear, SortDirection.Desc) => moviesQuery.OrderByDescending(movie => movie.ReleaseYear),
                (MovieSortBy.CreatedAt, SortDirection.Asc) => moviesQuery.OrderBy(movie => movie.CreatedAt),
                _ => moviesQuery.OrderByDescending(movie => movie.CreatedAt)
            };

            var totalCount = moviesQuery.Count();

            var items = moviesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Movie>(
                items,
                query.Page,
                query.PageSize,
                totalCount));
        }

        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_movies.FirstOrDefault(movie => movie.Id == id));
        }

        public Task<Movie?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return GetByIdAsync(id, cancellationToken);
        }

        public Task AddAsync(Movie movie, CancellationToken cancellationToken)
        {
            _movies.Add(movie);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Movie movie, CancellationToken cancellationToken)
        {
            var index = _movies.FindIndex(existing => existing.Id == movie.Id);
            if (index >= 0)
            {
                _movies[index] = movie;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Movie movie, CancellationToken cancellationToken)
        {
            _movies.Remove(movie);
            return Task.CompletedTask;
        }
    }
}
