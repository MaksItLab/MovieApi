using MovieApi.Contracts.Auth;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Auth
{
    internal static class AuthMapper
    {
        public static AuthUserResponse ToResponse(User user)
        {
            return new AuthUserResponse(
                user.Id,
                user.Email,
                user.CreatedAt);
        }
    }
}
