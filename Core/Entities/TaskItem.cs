using Core.Entities.Base;

namespace Core.Entities;

public class TaskItem : BaseEntity<int>
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
}
