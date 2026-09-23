using Microsoft.EntityFrameworkCore;
using MovieApi.Core.Interfaces;
using MovieApi.Domain.Entities;
using MovieApi.Infrastructure.Postgres.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Infrastructure.Postgres.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MovieDbContext _dbContext;

        public UserRepository(MovieDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> GetByEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken)
        {
            return _dbContext.Users
                .FirstOrDefaultAsync(
                    user => user.NormalizedEmail == normalizedEmail,
                    cancellationToken);
        }

        public Task<bool> ExistsByEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken)
        {
            return _dbContext.Users
                .AnyAsync(
                    user => user.NormalizedEmail == normalizedEmail,
                    cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _dbContext.Users
                .FirstOrDefaultAsync(
                    user => user.Id == id,
                    cancellationToken);
        }
    }
}
