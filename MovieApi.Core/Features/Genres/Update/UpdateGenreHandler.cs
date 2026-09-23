using MovieApi.Contracts.Genres;
using MovieApi.Contracts.Movies;
using MovieApi.Core.Common;
using MovieApi.Core.Genres;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Genres.Update
{
    public sealed class UpdateGenreHandler
    {
        private readonly IGenreRepository _genres;

        public UpdateGenreHandler(IGenreRepository genres)
        {
            _genres = genres;
        }

        public async Task<OperationResult<GenreResponse>> HandleAsync(
            Guid id,
            UpdateGenreRequest request,
            CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return OperationResult<GenreResponse>.Failure(
                        OperationErrorType.Validation,
                        "Name is required.");
            }

            if (request.Name.Length > 100)
            {
                return OperationResult<GenreResponse>.Failure(
                        OperationErrorType.Validation,
                        "Name is must be less 100 symbols.");
            }

            var genre = await _genres.GetByIdAsync(id, token);

            if (genre is null)
            {
                return OperationResult<GenreResponse>.Failure(
                        OperationErrorType.NotFound,
                        $"Genre with id='{id}' was not found.");
            }

            genre.Name = request.Name;
            genre.UpdatedAt = DateTime.UtcNow;

            await _genres.UpdateAsync(genre, token);

            var response = GenreMapper.ToResponse(genre);

            return OperationResult<GenreResponse>.Success(response);
        }
    }
}
