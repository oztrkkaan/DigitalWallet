namespace Application.Services.Wallets.Models;

public sealed record WalletBalanceResponse
{
    public int CustomerId { get; init; }
    public decimal Balance { get; init; }
}
