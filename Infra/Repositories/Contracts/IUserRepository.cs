using Core.Entities;

namespace Infra.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<User?> GetAsync(long id, CancellationToken cancellationToken = default);
        Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default);
    }
}