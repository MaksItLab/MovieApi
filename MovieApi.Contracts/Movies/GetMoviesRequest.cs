using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Movies
{
    public sealed record GetMoviesRequest(
        string? Search, 
        int? FromYear, 
        int? ToYear, 
        string? SortBy, 
        string? SortDirection,
        int Page = 1,
        int PageSize = 10);
    
}
