using MovieApi.Contracts.Common;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IMovieRepository
    {
        Task<PagedResult<Movie>> GetAllAsync(MovieListQuery query, CancellationToken cancellationToken);

        Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Movie?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Movie?> GetWithMediaAsync(Guid id, CancellationToken cancellationToken);

        void DeleteMoviePoster(MoviePoster poster, CancellationToken cancellationToken);

        Task RemoveVideoAsync(MovieVideo video, CancellationToken cancellationToken);

        Task AddAsync(Movie movie, CancellationToken cancellationToken);

        Task UpdateAsync(Movie movie, CancellationToken cancellationToken);

        Task DeleteAsync(Movie movie, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
