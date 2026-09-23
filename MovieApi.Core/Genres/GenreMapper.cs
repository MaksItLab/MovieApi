using MovieApi.Contracts.Genres;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Genres
{
    internal static class GenreMapper
    {
        public static GenreResponse ToResponse(Genre genre)
        {
            return new GenreResponse(
                genre.Id,
                genre.Name,
                genre.CreatedAt,
                genre.UpdatedAt);
        }

        public static GenreShortResponse ToShortResponse(Genre genre)
        {
            return new GenreShortResponse(genre.Id, genre.Name);
        }
    }
}
