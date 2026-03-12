namespace Core.Entities.Base
{

    public abstract class BaseEntity<T> : Entity<T>, ISoftDelete
    {
        private DateTime _createDate;

        public DateTime CreationTime
        {
            get
            {
                if (_createDate == default) _createDate = DateTime.Now;
                return _createDate;
            }
            set => _createDate = value;
        }

        public long? CreatorUserId { get; set; }
        public DateTime? LastModifcationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
