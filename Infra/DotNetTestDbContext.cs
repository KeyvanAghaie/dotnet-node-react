using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DotNetTestDbContext : DbContext
    {
        public DotNetTestDbContext(DbContextOptions<DotNetTestDbContext> options)
          : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
       
    }
}
