using MovieApi.Contracts.Genres;
using MovieApi.Core.Common;
using MovieApi.Core.Genres;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Features.Genres.Create
{
    public sealed class CreateGenreHandler
    {
        private readonly IGenreRepository _genres;

        public CreateGenreHandler(IGenreRepository genres)
        {
            _genres = genres;
        }

        public async Task<OperationResult<GenreResponse>> HandleAsync(
            CreateGenreRequest request,
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

            var now = DateTime.UtcNow;

            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _genres.AddAsync(genre, token);

            var response = GenreMapper.ToResponse(genre);

            return OperationResult<GenreResponse>.Success(response);
        }

        
    }
}
