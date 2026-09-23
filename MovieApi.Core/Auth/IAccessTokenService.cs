using MovieApi.Contracts.Auth;
using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Auth
{
    public interface IAccessTokenService
    {
        AccessToken Create(User user);
    }
}
