using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.Tests;

public class FakeCartService : ICartService
{
    private readonly Dictionary<string, Cart> _tempCarts = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> TempUserIds => _tempCarts.Keys.ToList();

    public void Clear() => _tempCarts.Clear();

    public void SeedTempCart(string tempUserId)
    {
        _tempCarts[tempUserId] = new Cart
        {
            TempUserId = tempUserId,
            UpdatedDate = DateTime.UtcNow
        };
    }

    public async Task<bool> TDeleteByTempUserIdAsync(string tempUserId)
        => await Task.FromResult(_tempCarts.Remove(tempUserId));

    public void TInsert(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts[t.TempUserId] = t;
        }
    }

    public void TDelete(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts.Remove(t.TempUserId);
        }
    }

    public void TUpdate(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts[t.TempUserId] = t;
        }
    }

    public List<Cart> TGetList() => _tempCarts.Values.ToList();

    public Cart TGetById(int id) => throw new NotSupportedException();

    public Task<bool> TTransferCartAsync(string tempUserId, int appUserId) => throw new NotSupportedException();

    public Task<bool> TAddToCartAsync(string? tempUserId, int userId, int productId, int quantity, ProductSize size) => throw new NotSupportedException();

    public async Task<CartDetailDto?> TGetCartDetailsAsync(string? tempUserId, int userId)
    {
        Cart? cart = null;
        if (!string.IsNullOrWhiteSpace(tempUserId))
        {
            _tempCarts.TryGetValue(tempUserId, out cart);
        }

        cart ??= _tempCarts.Values.FirstOrDefault(c => c.AppUserId == userId);

        if (cart == null)
        {
            return await Task.FromResult<CartDetailDto?>(null);
        }

        var dto = new CartDetailDto
        {
            CartId = cart.CartId,
            TempUserId = cart.TempUserId,
            AppUserId = cart.AppUserId,
            UpdatedDate = cart.UpdatedDate,
            Items = cart.CartItems?.Select(ci => new CartItemDetailDto
            {
                CartItemId = ci.CartItemId,
                CartId = ci.CartId,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name ?? string.Empty,
                CategoryName = ci.Product?.Category?.Name ?? string.Empty,
                Price = ci.Product?.Price ?? 0m,
                DiscountAmount = ci.Product?.DiscountAmount,
                DiscountRate = ci.Product?.DiscountRate,
                Quantity = ci.Quantity,
                Size = ci.Size,
                PrimaryImageUrl = ci.Product?.ProductImages?.FirstOrDefault()?.Url,
                ImageUrls = ci.Product?.ProductImages?.Select(pi => pi.Url).ToList() ?? new List<string>()
            }).ToList() ?? new List<CartItemDetailDto>(),
            Coupons = cart.CartCoupons?.Select(cc => new CartCouponDetailDto
            {
                CouponId = cc.CouponId,
                Code = cc.Coupon?.Code ?? string.Empty,
                DiscountAmount = cc.Coupon?.DiscountAmount,
                DiscountPercentage = cc.Coupon?.DiscountPercentage,
                ExpirationDate = cc.Coupon?.ExpirationDate ?? DateTime.UtcNow,
                IsActive = cc.Coupon?.IsActive ?? false
            }).ToList() ?? new List<CartCouponDetailDto>()
        };

        return await Task.FromResult<CartDetailDto?>(dto);
    }

    public Task<bool> TDeleteCartItemAsync(string? tempUserId, int userId, int productId, ProductSize size) => throw new NotSupportedException();

    public Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode) => throw new NotSupportedException();

    public Task<bool> TRemoveCouponFromCart(string? tempUserId, int userId, string couponCode) => throw new NotSupportedException();

    public Task TValidateCartCoupons(string? tempUserId, int userId) => Task.CompletedTask;
}
