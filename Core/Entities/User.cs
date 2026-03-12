using Core.Entities.Base;

namespace Core.Entities;

public class User : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
