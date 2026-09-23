using Microsoft.EntityFrameworkCore;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Postgres.Persistence;

namespace MovieApi.Infrastructure.Postgres.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly MovieDbContext _dbContext;

        public GenreRepository(MovieDbContext db)
        {
            _dbContext = db;
        }

        public async Task<PagedResult<Genre>> GetAllAsync(
            GenreListQuery query,
            CancellationToken cancellationToken)
        {
            var genresQuery = _dbContext.Genres.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var pattern = $"%{query.Search.Trim()}%";
                genresQuery = genresQuery.Where(genre => EF.Functions.ILike(genre.Name, pattern));
            }

            genresQuery = (query.SortBy, query.SortDirection) switch
            {
                (GenreSortBy.Name, SortDirection.Asc) => genresQuery.OrderBy(genre => genre.Name),
                (GenreSortBy.Name, SortDirection.Desc) => genresQuery.OrderByDescending(genre => genre.Name),
                (GenreSortBy.CreatedAt, SortDirection.Asc) => genresQuery.OrderBy(genre => genre.CreatedAt),
                _ => genresQuery.OrderByDescending(genre => genre.CreatedAt)
            };

            var totalCount = await genresQuery.CountAsync(cancellationToken);

            var genres = await genresQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Genre>(
                genres,
                query.Page,
                query.PageSize,
                totalCount);
        }

        public async Task<IReadOnlyList<Genre>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            if (ids.Count == 0)
            {
                return [];
            }

            return await _dbContext.Genres
                .Where(genre => ids.Contains(genre.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<Genre?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Genres
                .FirstOrDefaultAsync(genre => genre.Id == id, cancellationToken);
        }

        public async Task<bool> HasMoviesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.MovieGenres
                .AnyAsync(movieGenre => movieGenre.GenreId == id, cancellationToken);
        }

        public async Task AddAsync(Genre genre, CancellationToken cancellationToken)
        {
            _dbContext.Genres.Add(genre);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Genre genre, CancellationToken cancellationToken)
        {
            _dbContext.Genres.Update(genre);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Genre genre, CancellationToken cancellationToken)
        {
            _dbContext.Genres.Remove(genre);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
