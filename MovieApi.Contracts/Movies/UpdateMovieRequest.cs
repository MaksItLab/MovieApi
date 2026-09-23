namespace MovieApi.Contracts.Movies
{
    public sealed record UpdateMovieRequest(
        string Title,
        string? Description,
        int ReleaseYear,
        int DurationMinutes,
        IReadOnlyList<Guid>? GenreIds,
        IReadOnlyList<Guid>? ActorIds);
}
