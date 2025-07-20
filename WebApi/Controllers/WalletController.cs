using Application.Services.Wallets;
using Application.Services.Wallets.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("wallet")]
public class WalletController : ControllerBase
{

    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> Balance([FromQuery] GetBalanceRequest request, CancellationToken ct)
    {
        var response = await _walletService.GetBalanceAsync(request, ct);

        return Ok(response);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(AddDepositRequest request, CancellationToken ct)
    {
        await _walletService.AddDepositAsync(request, ct);

        return Ok();
    }
}
