using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Base
{
    
    public abstract class EntityDto<T> : BaseEntityDto<T>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public long? CreatorUserId { get; set; }
        public long UserId { get; set; }
        public DateTime? LastModifcationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
