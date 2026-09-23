using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Auth
{
    public sealed record AuthUserResponse(
        Guid Id,
        string Email,
        DateTime CreatedAt);
    
}
