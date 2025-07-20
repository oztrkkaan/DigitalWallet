namespace Application.Services.Shoppings.Models;

public sealed record PurchaseRequest(int CustomerId, int CartId);
