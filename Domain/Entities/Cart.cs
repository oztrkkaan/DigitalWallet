using Domain.Common;

namespace Domain.Entities;
public class Cart : AuditableEntity
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public CartStatus Status { get; set; };

    public Customer Customer { get; set; }

}

public enum CartStatus
{
    Active,
    Paid,
    Cancelled
}
