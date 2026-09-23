using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Movies
{
    public record CreateMovieRequest(
        string Title, 
        string? Description,
        int ReleaseYear, 
        int DurationMinutes,
        IReadOnlyList<Guid>? GenreIds,
        IReadOnlyList<Guid>? ActorIds);
    
}
