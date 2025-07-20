using Application.Services.Wallets.Models;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence.SqlConnection;

namespace Application.Services.Wallets;

public interface IWalletService
{
    Task AddDepositAsync(AddDepositRequest request, CancellationToken ct = default);
    Task<WalletBalanceResponse> GetBalanceAsync(GetBalanceRequest request, CancellationToken ct = default);

}
public class WalletService : IWalletService
{
    private readonly SqlConnectionFactory _connectionFactory;
    public WalletService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task AddDepositAsync(AddDepositRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0) throw new ArgumentException("Deposit amount must be greater than zero.");

        var connection = _connectionFactory.Create();

        const string getWalletSql = @"SELECT * FROM Wallet WHERE CustomerId = @CustomerId";

        const string updateWalletSql = @"UPDATE Wallet SET Balance = @NewBalance WHERE Id = @WalletId";

        const string insertTransactionSql = @"
            INSERT INTO WalletTransaction (WalletId, Type, Amount, BeforeBalance, AfterBalance, CreatedAt)
            VALUES (@WalletId, @Type, @Amount, @BeforeBalance, @AfterBalance, GETUTCDATE())";

        using var transaction = connection.BeginTransaction();

        try
        {
            var wallet = await connection.QuerySingleOrDefaultAsync<Wallet>(
                getWalletSql,
                new { request.CustomerId },
                transaction: transaction
            );

            if (wallet == null)
                throw new Exception("Wallet not found for the customer.");


            wallet.AddDeposit(request.Amount);

            await connection.ExecuteAsync(
                updateWalletSql,
                new
                {
                    NewBalance = wallet.Balance,
                    WalletId = wallet.Id
                },
                transaction: transaction
            );

            var walletTransaction = wallet.WalletTransactions.FirstOrDefault();

            await connection.ExecuteAsync(
                insertTransactionSql,
                new
                {
                    WalletId = wallet.Id,
                    Type = (int)walletTransaction.Type,
                    Amount = request.Amount,
                    BeforeBalance = walletTransaction.BeforeBalance,
                    AfterBalance = walletTransaction.AfterBalance,
                },
                transaction: transaction
            );

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<WalletBalanceResponse> GetBalanceAsync(GetBalanceRequest request, CancellationToken ct = default)
    {
        var connection = _connectionFactory.Create();

        const string sql = @"SELECT CustomerId, Balance FROM Wallet WHERE CustomerId = @CustomerId";

        var result = await connection.QuerySingleOrDefaultAsync<WalletBalanceResponse>(
            sql,
            new { CustomerId = request.CustomerId });

        if (result == null)
            throw new KeyNotFoundException($"Wallet not found for customerId={request.CustomerId}");

        return result;
    }

}
