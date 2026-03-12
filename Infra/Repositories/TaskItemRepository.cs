using Core.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly DotNetTestDbContext _dbContext;
        public TaskItemRepository(DotNetTestDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<TaskItem?> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TaskItems.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.TaskItems.ToListAsync(cancellationToken);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            await _dbContext.TaskItems.AddAsync(task, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return task;
        }

        public async Task<TaskItem?> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
        {
            var existingTask = await _dbContext.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskItem.Id, cancellationToken);

            if (existingTask is null)
                return null;

            existingTask.Title = taskItem.Title;
            existingTask.Status = taskItem.Status;
            existingTask.UserId = taskItem.UserId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingTask;
        }

    }
}
