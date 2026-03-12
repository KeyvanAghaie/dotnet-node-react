using Core.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DotNetTestDbContext _dbContext;
        public UserRepository(DotNetTestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.ToListAsync(cancellationToken);
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(c => c.Id == user.Id, cancellationToken);

            if (existingUser is null)
                return null;

            // Update properties
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingUser;
        }

    }
}
