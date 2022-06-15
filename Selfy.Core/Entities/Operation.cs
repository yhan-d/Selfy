using Selfy.Core.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfy.Core.Entities
{
    public class Operation : BaseEntity<int>
    {
        public int RequestId { get; set; }
        public Request? Request { get; set; }

        public string? UserId { get; set; }
    }
}
