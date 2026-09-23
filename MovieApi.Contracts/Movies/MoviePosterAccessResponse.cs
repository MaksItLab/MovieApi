using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Movies
{
    public sealed record MoviePosterAccessResponse(
        string Url,
        DateTimeOffset ExpiresAtUtc);
}
