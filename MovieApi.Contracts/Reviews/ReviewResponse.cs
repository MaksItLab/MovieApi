using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Reviews
{
    public sealed record ReviewResponse(
        Guid Id,
        Guid MovieId,
        Guid AuthorId,
        string Text,
        int Rating,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
