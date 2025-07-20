using Domain.Common;

namespace Domain.Entities;

public class Customer : AuditableEntity
{

    public Customer(string name, string email)
    {
        SetName(name);
        SetEmail(email);
    }

    private Customer() { }
    public int Id { get; set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Wallet Wallet { get; set; }

    public void SetName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException($"'{nameof(Name)}' can not be null.");
        }

        Name = name;
    }

    public void SetEmail(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException($"'{nameof(Email)}' can not be null.");
        }

        Email = email;
    }
}
