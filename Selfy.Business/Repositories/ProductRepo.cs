using Selfy.Business.Repositories.Abstracts.EntityFrameworkCore;
using Selfy.Core.Entities;
using Selfy.Data.EntityFramework;

namespace Selfy.Business.Repositories
{
    public class ProductRepo : RepositoryBase<Product, int>
    {
        public ProductRepo(MyContext context) : base(context)
        {
        }
    }
}
