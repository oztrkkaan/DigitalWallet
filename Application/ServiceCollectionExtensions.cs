using Application.Services.Carts;
using Application.Services.Wallets;
using Infrastructure.Persistence.SqlConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                    throw new ApplicationException("The connection string is null");
            return new SqlConnectionFactory(connectionString).Create();
        });

        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ICartService, CartService>();
    }
}
