using Selfy.Core.Entities.Abstracts;

namespace Selfy.Core.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; }

    public IList<Brand>? Brands { get; set; }
}