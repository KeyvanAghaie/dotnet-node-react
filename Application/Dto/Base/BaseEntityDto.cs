using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Base
{
    [Serializable]
    public class BaseEntityDto<TPrimaryKey>
    {
        public TPrimaryKey? Id { get; set; }
    }
}
