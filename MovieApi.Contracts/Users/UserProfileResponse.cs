using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Users
{
    public sealed record UserProfileResponse(
        Guid Id,
        string Email,
        DateTime CreatedAt);
}
