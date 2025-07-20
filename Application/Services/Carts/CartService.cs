using Domain.Entities;
using Infrastructure.Persistence.SqlConnection;
using Dapper;
using Application.Services.Shoppings.Models;

namespace Application.Services.Carts;

public interface ICartService
{
    Task PurchaseAsync(PurchaseRequest request, CancellationToken ct = default);

}
public class CartService : ICartService
{
    private readonly SqlConnectionFactory _connectionFactory;
    public CartService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task PurchaseAsync(PurchaseRequest request, CancellationToken ct = default)
    {
        var connection = _connectionFactory.Create();
        using var transaction = connection.BeginTransaction();
        try
        {
            var cart = await connection.QuerySingleOrDefaultAsync<Cart>(
                "SELECT * FROM Cart WHERE Id = @CartId AND CustomerId = @CustomerId",
                new { request.CartId, request.CustomerId },
                transaction
            );

            if (cart == null)
                throw new Exception("Cart not found.");

            if (cart.Status == CartStatus.Paid)
                throw new Exception("Cart has already been paid.");

            var items = (await connection.QueryAsync<CartItem>(
                "SELECT * FROM CartItem WHERE CartId = @CartId",
                new { request.CartId },
                transaction
            )).ToList();

            if (!items.Any())
                throw new Exception("Cart is empty.");

            var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice);

            var wallet = await connection.QuerySingleOrDefaultAsync<Wallet>(
                "SELECT * FROM Wallet WHERE CustomerId = @CustomerId",
                new { request.CustomerId },
                transaction
            );

            if (wallet == null)
                throw new Exception("Wallet not found.");

            if (wallet.Balance < totalAmount)
                throw new Exception("Insufficient wallet balance.");

            var beforeBalance = wallet.Balance;
            var afterBalance = beforeBalance - totalAmount;

            await connection.ExecuteAsync(
                "UPDATE Wallet SET Balance = @Balance WHERE Id = @WalletId",
                new { Balance = afterBalance, WalletId = wallet.Id },
                transaction
            );

            await connection.ExecuteAsync(
                @"INSERT INTO WalletTransaction (WalletId, Type, Amount, BeforeBalance, AfterBalance, CreatedAt)
                  VALUES (@WalletId, @Type, @Amount, @BeforeBalance, @AfterBalance, GETUTCDATE())",
                new
                {
                    WalletId = wallet.Id,
                    Type = (int)WalletTransactionType.Outcoming,
                    Amount = totalAmount,
                    BeforeBalance = beforeBalance,
                    AfterBalance = afterBalance
                },
                transaction
            );

            await connection.ExecuteAsync(
                "UPDATE Cart SET Status = 'Paid' WHERE Id = @CartId",
                new { request.CartId },
                transaction
            );

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

    }
}
