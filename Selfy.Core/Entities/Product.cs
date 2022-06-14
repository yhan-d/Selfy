using Selfy.Core.Entities.Abstracts;

namespace Selfy.Core.Entities;

public class Product : BaseEntity<int>
{
    public string Name { get; set; }

    public IList<Request>? Requests { get; set; }
}