using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Genres
{
    public sealed record GenreShortResponse(Guid Id, string Name);
}
