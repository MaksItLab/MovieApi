using MovieApi.Contracts.Common;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Genres;
using MovieApi.Core.Reviews;
using MovieApi.Domain.Entities;

namespace MovieApi.Core.Movies
{
    internal static class MovieMapper
    {
        public static MovieDetailsResponse ToResponse(Movie movie)
        {
            var genres = movie.MovieGenres
                .Where(movieGenre => movieGenre.Genre is not null)
                .Select(movieGenre => GenreMapper.ToShortResponse(movieGenre.Genre))
                .ToList();

            var actors = movie.MovieActors
                .Where(movieActor => movieActor.Actor is not null)
                .Select(movieActor => ActorMapper.ToShortResponse(movieActor.Actor))
                .ToList();

            var reviews = movie.Reviews
                .Select(ReviewMapper.ToResponse)
                .ToList();

            return new MovieDetailsResponse(
                movie.Id,
                movie.Title,
                movie.Description,
                movie.ReleaseYear,
                movie.DurationMinutes,
                movie.CreatedAt,
                movie.UpdatedAt,
                genres,
                actors,
                reviews);
        }

        public static MovieListItemResponse ToListItemResponse(Movie movie)
        {
            return new MovieListItemResponse(
                movie.Id,
                movie.Title,
                movie.ReleaseYear,
                movie.DurationMinutes);
        }

        public static PagedResponse<MovieListItemResponse> ToPagedResponse(PagedResult<Movie> result)
        {
            var items = result.Items
                .Select(ToListItemResponse)
                .ToList();

            return new PagedResponse<MovieListItemResponse>(
                items,
                result.Page,
                result.PageSize,
                result.TotalCount,
                result.TotalPages);
        }
    }
}
