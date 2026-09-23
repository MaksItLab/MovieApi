using MovieApi.Core.Common;
using MovieApi.Core.Interfaces;

namespace MovieApi.Core.Features.Genres.Delete
{
    public sealed class DeleteGenreHandler
    {
        private readonly IGenreRepository _genres;

        public DeleteGenreHandler(IGenreRepository genres)
        {
            _genres = genres;
        }

        public async Task<OperationResult> HandleAsync(
            Guid id,
            CancellationToken token)
        {
            var genre = await _genres.GetByIdAsync(id, token);

            if (genre is null)
            {
                return OperationResult.Failure(
                    OperationErrorType.NotFound,
                    $"Genre with id='{id}' was not found.");
            }

            if (await _genres.HasMoviesAsync(id, token))
            {
                return OperationResult.Failure(
                    OperationErrorType.Conflict,
                    "Genre is linked to movies and cannot be deleted.");
            }

            await _genres.DeleteAsync(genre, token);

            return OperationResult.Success();
        }
    }
}
