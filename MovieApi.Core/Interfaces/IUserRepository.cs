using MovieApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

        Task AddAsync(User user, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);

        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
