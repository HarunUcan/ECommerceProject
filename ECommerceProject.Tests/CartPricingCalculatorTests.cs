using System;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;
using Xunit;

namespace ECommerceProject.Tests;

public class CartPricingCalculatorTests
{
    [Fact]
    public void CalculateTotals_AppliesProductDiscountsAndCoupons()
    {
        var cart = new CartDetailDto
        {
            Items =
            {
                new CartItemDetailDto
                {
                    ProductId = 1,
                    Price = 100m,
                    DiscountRate = 10m,
                    Quantity = 2,
                    Size = ProductSize.M,
                },
                new CartItemDetailDto
                {
                    ProductId = 2,
                    Price = 50m,
                    DiscountAmount = 5m,
                    Quantity = 1,
                    Size = ProductSize.S,
                }
            },
            Coupons =
            {
                new CartCouponDetailDto
                {
                    CouponId = 1,
                    Code = "PERCENT10",
                    DiscountPercentage = 10m,
                    IsActive = true,
                    ExpirationDate = DateTime.UtcNow.AddDays(1)
                }
            }
        };

        var result = CartPricingCalculator.CalculateTotals(cart);

        Assert.Equal(225m, result.Subtotal);
        Assert.Equal(22.5m, result.CouponDiscount, 1);
        Assert.Equal(202.5m, result.FinalTotal);
        Assert.Equal(22.5m, result.Tax);
    }

    [Fact]
    public void CalculateTotals_ReturnsZero_WhenCartNull()
    {
        var result = CartPricingCalculator.CalculateTotals(null);

        Assert.Equal(0m, result.Subtotal);
        Assert.Equal(0m, result.CouponDiscount);
        Assert.Equal(0m, result.FinalTotal);
    }
}
