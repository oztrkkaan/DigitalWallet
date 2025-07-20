using Domain.Common;

namespace Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
}
