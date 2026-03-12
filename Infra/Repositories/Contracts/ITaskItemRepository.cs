using Core.Entities;

namespace Infra.Repositories
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TaskItem?> GetAsync(long id, CancellationToken cancellationToken = default);
        Task<TaskItem?> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    }
}