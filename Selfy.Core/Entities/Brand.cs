using Selfy.Core.Entities.Abstracts;

namespace Selfy.Core.Entities;

public class Brand : BaseEntity<int>
{
    public string Name { get; set; }
    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}