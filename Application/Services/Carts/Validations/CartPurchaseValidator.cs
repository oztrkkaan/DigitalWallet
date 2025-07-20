using Application.Services.Shoppings.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Carts.Validations
{
    public sealed class CartPurchaseValidator  : AbstractValidator<PurchaseRequest>
    {
        public CartPurchaseValidator()
        {
            RuleFor(m => m.CartId)
               .GreaterThan(0).WithMessage($"Invalid CartId");


            RuleFor(m => m.CustomerId)
               .GreaterThan(0).WithMessage($"Invalid CustomerId");
        }
    }
}
