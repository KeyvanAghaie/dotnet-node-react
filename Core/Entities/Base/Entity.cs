namespace Core.Entities.Base
{
    public class Entity<TPrimaryKey>
    {
        //
        // Summary:
        //     Unique identifier for this entity.
        public virtual required TPrimaryKey Id { get; set; }
    }
}
