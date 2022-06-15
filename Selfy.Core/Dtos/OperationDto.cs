using Selfy.Core.Dtos.Abstract;
using Selfy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfy.Core.Dtos
{
    public class OperationDto : BaseDto<int>
    {
        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public string? UserId { get; set; }
    }
}
