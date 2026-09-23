using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Actors
{
    public sealed record ActorResponse(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime? BirthDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);
}
