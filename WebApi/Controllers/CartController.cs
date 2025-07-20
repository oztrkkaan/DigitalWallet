using Application.Services.Carts;
using Application.Services.Shoppings.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IValidator<PurchaseRequest> _purchaheValidator;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request, CancellationToken ct)
    {
        var validationReuslt = _purchaheValidator.Validate(request);
        
        if (!validationReuslt.IsValid)
        {
            return BadRequest(validationReuslt.Errors);
        }

        await _cartService.PurchaseAsync(request, ct);

        return Ok();
    }
}
