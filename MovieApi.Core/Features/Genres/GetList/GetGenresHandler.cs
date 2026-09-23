using MovieApi.Contracts.Common;
using MovieApi.Contracts.Genres;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Genres;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Genres.GetList
{
    public class GetGenresHandler
    {
        private readonly IGenreRepository _genres;

        public GetGenresHandler(IGenreRepository genres)
        {
            _genres = genres;
        }

        public async Task<OperationResult<PagedResponse<GenreResponse>>> HandleAsync(
            GetGenresRequest request,
            CancellationToken token)
        {
            if (request.Page <= 0)
            {
                return OperationResult<PagedResponse<GenreResponse>>.Failure(
                    OperationErrorType.Validation,
                    "page cannot be less than 1");
            }

            if (request.PageSize <= 0 ||
                request.PageSize > 50)
            {
                return OperationResult<PagedResponse<GenreResponse>>.Failure(
                    OperationErrorType.Validation,
                    "pageSize cannot be greater than 50 and less 1");
            }

            TryParseSortBy(request.SortBy, out GenreSortBy sortBy);
            TryParseSortDirection(request.SortDirection, out SortDirection sortDirection);

            var query = new GenreListQuery(
                request.Search,
                sortBy,
                sortDirection,
                request.Page,
                request.PageSize
                );

            var genres = await _genres.GetAllAsync(query, token);

            var items = genres.Items
                .Select(GenreMapper.ToResponse)
                .ToList();

            var totalPages = genres.TotalCount == 0
                ? 0
                : (int)Math.Ceiling(genres.TotalCount / (double)request.PageSize);

            var response = new PagedResponse<GenreResponse>(
                items,
                query.Page,
                query.PageSize,
                genres.TotalCount,
                totalPages);

            return OperationResult<PagedResponse<GenreResponse>>.Success(response);
        }

        private static bool TryParseSortBy(string? value, out GenreSortBy sortBy)
        {
            sortBy = GenreSortBy.CreatedAt;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var normalized = value.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "name":
                    sortBy = GenreSortBy.Name;
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

    public sealed record GenreListQuery(
       string? Search,
       GenreSortBy SortBy,
       SortDirection SortDirection,
       int Page = 1,
       int PageSize = 10);

    public enum SortDirection
    {
        Asc,
        Desc
    }

    public enum GenreSortBy
    {
        CreatedAt,
        Name
    }
}
