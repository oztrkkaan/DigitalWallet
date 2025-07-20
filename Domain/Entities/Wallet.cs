using Domain.Common;

namespace Domain.Entities;

public class Wallet : AuditableEntity
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public ICollection<WalletTransaction> WalletTransactions { get; set; }

    public void AddDeposit(decimal amount)
    {
        var last = WalletTransactions
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        WalletTransaction walletTransaction = new(this, amount, last);
        WalletTransactions.Add(walletTransaction);

        Balance = walletTransaction.AfterBalance;
    }
}
