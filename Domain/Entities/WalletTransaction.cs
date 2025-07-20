using Domain.Common;

namespace Domain.Entities;

public class WalletTransaction : AuditableEntity
{
    public WalletTransaction(Wallet wallet, decimal amount, WalletTransaction? lastWalletTransaction)
    {
        WalletId = wallet.Id;
        Amount = amount;

        if (lastWalletTransaction is not null)
        {
            BeforeBalance = lastWalletTransaction.AfterBalance;
            AfterBalance = lastWalletTransaction.AfterBalance + amount;
        }
        else
        {
            BeforeBalance = 0;
            AfterBalance = amount;
        }


        if (amount > 0)
        {
            Type = WalletTransactionType.Incoming;
        }
        else if (amount < 0)
        {
            Type = WalletTransactionType.Outcoming;
        }
        else
        {
            throw new ArgumentException($"'{nameof(Amount)}' cannot be zero.");
        }
    }

    private WalletTransaction() { }

    public int Id { get; set; }
    public int WalletId { get; private set; }
    public WalletTransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal BeforeBalance { get; private set; }
    public decimal AfterBalance { get; private set; }

}
public enum WalletTransactionType
{
    Incoming = 1,
    Outcoming = 2,
}
