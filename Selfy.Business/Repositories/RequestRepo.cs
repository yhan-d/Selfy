using Selfy.Business.Repositories.Abstracts.EntityFrameworkCore;
using Selfy.Core.Entities;
using Selfy.Data.EntityFramework;
namespace Selfy.Business.Repositories
{
    public class RequestRepo : RepositoryBase<Request, int>
    {
        public RequestRepo(MyContext context) : base(context)
        {

        }
    }
}
