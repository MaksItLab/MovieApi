using MovieApi.Contracts.Genres;
using MovieApi.Core.Common;
using MovieApi.Core.Genres;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Core.Features.Genres.GetById
{
    public sealed class GetGenreByIdHandler
    {
        private readonly IGenreRepository _genres;

        public GetGenreByIdHandler(IGenreRepository genres)
        {
            _genres = genres;
        }

        public async Task<OperationResult<GenreResponse>> HandleAsync(
            Guid id,
            CancellationToken token)
        {
            var genre = await _genres.GetByIdAsync(id, token);

            if (genre is null)
            {
                return OperationResult<GenreResponse>.Failure(
                        OperationErrorType.NotFound,
                        $"Genre with id='{id}' was not found.");
            }

            var response = GenreMapper.ToResponse(genre);

            return OperationResult<GenreResponse>.Success(response);
        }
    }
}
