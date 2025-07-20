namespace Application.Services.Wallets.Models;

public sealed record AddDepositRequest(decimal Amount, int CustomerId);
