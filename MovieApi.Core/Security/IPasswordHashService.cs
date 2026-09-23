using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Security
{
    public interface IPasswordHashService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(User user, string password);
    }
}
