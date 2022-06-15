using Selfy.Business.Repositories.Abstracts.EntityFrameworkCore;
using Selfy.Core.Entities;
using Selfy.Data.EntityFramework;

namespace Selfy.Business.Repositories
{
    public class OperationRepo : RepositoryBase<Operation, int>
    {
        public OperationRepo(MyContext context) : base(context)
        {
        }
    }
}
