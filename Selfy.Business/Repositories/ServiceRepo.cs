using Selfy.Business.Repositories.Abstracts.EntityFrameworkCore;
using Selfy.Core.Entities;
using Selfy.Data.EntityFramework;

namespace Selfy.Business.Repositories
{
    public class ServiceRepo : RepositoryBase<Service, int>
    {
        public ServiceRepo(MyContext context) : base(context)
        {

        }
    }
}
