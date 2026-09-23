using MovieApi.Contracts.Common;
using MovieApi.Contracts.Genres;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;
using MovieApi.Core.Movies;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Movies.GetList
{
    public sealed class GetMoviesHandler
    {
        private readonly IMovieRepository _movies;

        public GetMoviesHandler(IMovieRepository movies)
        {
            _movies = movies;
        }

        public async Task<OperationResult<PagedResponse<MovieListItemResponse>>> HandleAsync(
            GetMoviesRequest request,
            CancellationToken token)
        {
            if (request.FromYear.HasValue &&
                request.ToYear.HasValue &&
                request.FromYear > request.ToYear)
            {
                return OperationResult<PagedResponse<MovieListItemResponse>>.Failure(
                    OperationErrorType.Validation,
                    "fromYear cannot be greater than toYear.");
            }

            if (request.Page <= 0)
            {
                return OperationResult<PagedResponse<MovieListItemResponse>>.Failure(
                    OperationErrorType.Validation,
                    "page cannot be less than 1");
            }

            if (request.PageSize <= 0 ||
                request.PageSize > 50)
            {
                return OperationResult<PagedResponse<MovieListItemResponse>>.Failure(
                    OperationErrorType.Validation,
                    "pageSize cannot be greater than 50 and less 1");
            }

            TryParseSortBy(request.SortBy, out MovieSortBy sortBy);
            TryParseSortDirection(request.SortDirection, out SortDirection sortDirection);

            var query = new MovieListQuery(
                request.Search,
                request.FromYear,
                request.ToYear,
                sortBy,
                sortDirection,
                request.Page,
                request.PageSize);

            var movies = await _movies.GetAllAsync(query, token);

            return OperationResult<PagedResponse<MovieListItemResponse>>
                .Success(MovieMapper.ToPagedResponse(movies));
        }

        private static bool TryParseSortBy(string? value, out MovieSortBy sortBy)
        {
            sortBy = MovieSortBy.CreatedAt;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var normalized = value.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "title":
                    sortBy = MovieSortBy.Title;
                    return true;

                case "releaseyear":
                    sortBy = MovieSortBy.ReleaseYear;
                    return true;

                case "createdat":
                    sortBy = MovieSortBy.CreatedAt;
                    return true;

                default:
                    return false;
            }
        }

        private static bool TryParseSortDirection(
            string? value,
            out SortDirection sortDirection)
        {
            sortDirection = SortDirection.Desc;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var normalized = value.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "asc":
                    sortDirection = SortDirection.Asc;
                    return true;

                case "desc":
                    sortDirection = SortDirection.Desc;
                    return true;

                default:
                    return false;
            }
        }
    }

    public enum MovieSortBy
    {
        CreatedAt,
        Title,
        ReleaseYear
    }

    public enum SortDirection
    {
        Asc,
        Desc
    }

    public sealed record MovieListQuery(
       string? Search,
       int? FromYear,
       int? ToYear,
       MovieSortBy SortBy,
       SortDirection SortDirection,
       int Page = 1,
       int PageSize = 10);
}
