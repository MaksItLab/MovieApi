using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Genres
{
    public sealed record GenreResponse(
        Guid Id,
        string Name,
        DateTime CreatedAt,
        DateTime UpdatedAt
        );
}
