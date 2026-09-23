using MovieApi.Contracts.Actors;
using MovieApi.Contracts.Genres;
using MovieApi.Contracts.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Movies
{
    public sealed record MovieDetailsResponse(
        Guid Id,
        string Title,
        string? Description,
        int ReleaseYear,
        int DurationMinutes,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        IReadOnlyList<GenreShortResponse> Genres,
        IReadOnlyList<ActorShortResponse> Actors,
        IReadOnlyList<ReviewResponse> Reviews);
}
