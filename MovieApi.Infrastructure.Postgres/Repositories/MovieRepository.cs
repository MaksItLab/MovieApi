using Microsoft.EntityFrameworkCore;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Postgres.Persistence;

namespace MovieApi.Infrastructure.Postgres.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _dbContext;

        public MovieRepository(MovieDbContext db)
        {
            _dbContext = db;
        }

        public async Task<PagedResult<Movie>> GetAllAsync(
            MovieListQuery query,
            CancellationToken cancellationToken)
        {
            var moviesQuery = _dbContext.Movies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var pattern = $"%{query.Search.Trim()}%";

                moviesQuery = moviesQuery.Where(movie =>
                    EF.Functions.ILike(movie.Title, pattern)
                    || (movie.Description != null
                        && EF.Functions.ILike(movie.Description, pattern)));
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

            var totalCount = await moviesQuery.CountAsync(cancellationToken);

            var movies = await moviesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Movie>(
                movies,
                query.Page,
                query.PageSize,
                totalCount);
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Movies
                .FirstOrDefaultAsync(movie => movie.Id == id, cancellationToken);
        }

        public async Task<Movie?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Movies
                .Include(movie => movie.MovieGenres)
                .ThenInclude(movieGenre => movieGenre.Genre)
                .Include(movie => movie.MovieActors)
                .ThenInclude(movieActor => movieActor.Actor)
                .Include(movie => movie.Reviews)
                .FirstOrDefaultAsync(movie => movie.Id == id, cancellationToken);
        }

        public async Task AddAsync(Movie movie, CancellationToken cancellationToken)
        {
            await _dbContext.Movies.AddAsync(movie, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Movie movie, CancellationToken cancellationToken)
        {
            _dbContext.Movies.Update(movie);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Movie movie, CancellationToken cancellationToken)
        {
            _dbContext.Movies.Remove(movie);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Movie?> GetWithMediaAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _dbContext.Movies
                .Include(movie => movie.Poster)
                .Include(movie => movie.Video)
                .FirstOrDefaultAsync(movie => movie.Id == id, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void DeleteMoviePoster(MoviePoster poster, CancellationToken cancellationToken)
        {
            _dbContext.MoviePosters.Remove(poster);
        }

        public Task RemoveVideoAsync(MovieVideo video, CancellationToken cancellationToken)
        {
            _dbContext.MovieVideos.Remove(video);
            return Task.CompletedTask;
        }
    }
}
