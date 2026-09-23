using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Contracts.Auth
{
    public sealed record LoginResponse(
        string AccessToken,
        string TokenType,
        DateTime ExpiresAtUtc,
        AuthUserResponse User);
}
