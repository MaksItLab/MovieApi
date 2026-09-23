using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Actors
{
    public sealed record UpdateActorRequest(
    string FirstName,
    string LastName,
    DateTime? BirthDate);
}
