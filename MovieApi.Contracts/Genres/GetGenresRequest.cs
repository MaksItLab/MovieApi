using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Genres
{
    public sealed record GetGenresRequest(
        string? Search,
        string? SortBy,
        string? SortDirection,
        int Page = 1,
        int PageSize = 10
        );
}
