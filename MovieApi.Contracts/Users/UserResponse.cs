using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Users
{
    public sealed record UserResponse(
        Guid Id,
        string Email
        );
}

// dotnet ef migrations add AddUsers --project MovieApi.Infrastructure.Postgres --startup-project MovieApi.Web
// dotnet ef database update --project MovieApi.Infrastructure.Postgres --startup-project MovieApi.Web