using ECommerceProject.DtoLayer.Dtos.CartDtos;

namespace ECommerceProject.BusinessLayer.Helpers;

public static class CartPricingCalculator
{
    public static CartPricingResult CalculateTotals(CartDetailDto? cart)
    {
        if (cart == null)
        {
            return new CartPricingResult(0m, 0m, 0m);
        }

        decimal subtotal = 0m;
        foreach (var item in cart.Items)
        {
            decimal unitPrice = item.Price;
            if (item.DiscountRate.HasValue)
            {
                unitPrice -= unitPrice * item.DiscountRate.Value / 100m;
            }
            else if (item.DiscountAmount.HasValue)
            {
                unitPrice -= item.DiscountAmount.Value;
            }

            subtotal += unitPrice * item.Quantity;
        }

        decimal couponDiscount = 0m;
        foreach (var coupon in cart.Coupons)
        {
            if (coupon.DiscountPercentage.HasValue)
            {
                couponDiscount += subtotal * coupon.DiscountPercentage.Value / 100m;
            }
            else if (coupon.DiscountAmount.HasValue)
            {
                couponDiscount += coupon.DiscountAmount.Value;
            }
        }

        if (couponDiscount > subtotal)
        {
            couponDiscount = subtotal;
        }

        var finalTotal = subtotal - couponDiscount;
        return new CartPricingResult(subtotal, couponDiscount, finalTotal);
    }
}

public record CartPricingResult(decimal Subtotal, decimal CouponDiscount, decimal FinalTotal)
{
    public decimal Tax => Math.Round(Subtotal * 0.1m, 2);
}
