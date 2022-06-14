using Selfy.Core.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfy.Core.Entities
{
    public class Request : BaseEntity<int>
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string TextOfRequest { get; set; }

        public string Address { get; set; }

        public Boolean Status { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

    }
}
