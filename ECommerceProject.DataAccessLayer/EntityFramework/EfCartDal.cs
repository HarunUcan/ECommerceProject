using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProject.DataAccessLayer.EntityFramework;

public class EfCartDal : GenericRepository<Cart>, ICartDal
{
    public async Task<bool> AddToCartAsync(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
    {
        await using var context = new Context();

        Cart? cart;
        if (!string.IsNullOrWhiteSpace(tempUserId))
        {
            cart = await context.Carts.FirstOrDefaultAsync(x => x.TempUserId == tempUserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    TempUserId = tempUserId,
                    UpdatedDate = DateTime.UtcNow
                };
                await context.Carts.AddAsync(cart);
                await context.SaveChangesAsync();
            }
        }
        else
        {
            cart = await context.Carts.FirstOrDefaultAsync(x => x.AppUserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    AppUserId = userId,
                    UpdatedDate = DateTime.UtcNow
                };
                await context.Carts.AddAsync(cart);
                await context.SaveChangesAsync();
            }
        }

        if (cart == null)
        {
            return false;
        }

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = productId,
                Quantity = quantity,
                Size = size
            };
            await context.CartItems.AddAsync(cartItem);
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        cart.UpdatedDate = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApplyCoupon(string? tempUserId, int userId, string couponCode)
    {
        await using var context = new Context();

        var coupon = await context.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode);
        if (coupon == null)
        {
            throw new Exception("Bu kupon kodu geçersiz!");
        }

        if (!coupon.IsActive)
        {
            throw new Exception("Bu kupon kodu geçersiz!");
        }

        if (!string.IsNullOrWhiteSpace(tempUserId))
        {
            throw new Exception("İndirimlerden yararlanmak için kayıt olmalı ve giriş yapmalısınız!");
        }

        var query = context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .Include(c => c.CartCoupons)
                .ThenInclude(cc => cc.Coupon);

        Cart? cart = !string.IsNullOrWhiteSpace(tempUserId)
            ? await query.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
            : await query.FirstOrDefaultAsync(c => c.AppUserId == userId);

        if (cart == null)
        {
            return false;
        }

        if (coupon.ExpirationDate < DateTime.Now)
        {
            throw new Exception("Bu kuponun son kullanma tarihi geçmiştir!");
        }

        bool alreadyApplied = cart.CartCoupons?.Any(cc => cc.CouponId == coupon.CouponId) ?? false;
        if (alreadyApplied)
        {
            throw new Exception("Bu kupon zaten kullanılıyor!");
        }

        bool cartHasAnyCoupons = cart.CartCoupons?.Any() ?? false;
        if (cartHasAnyCoupons)
        {
            throw new Exception("Sepette yalnızca 1 kupon kullanabilirsiniz!");
        }

        if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
        {
            throw new Exception("Bu kuponun kullanım limiti dolmuştur!");
        }

        var cartTotalPrice = cart.CartItems?.Sum(ci => ci.Quantity * ci.Product.Price);
        if (cartTotalPrice < coupon.MinOrderAmount)
        {
            throw new Exception("Bu kuponu kullanmak için olan minimum sipariş tutarı sağlanmamıştır!");
        }

        var cartCoupon = new CartCoupon
        {
            CartId = cart.CartId,
            CouponId = coupon.CouponId
        };

        await context.CartCoupons.AddAsync(cartCoupon);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByTempUserIdAsync(string tempUserId)
    {
        await using var context = new Context();
        var cart = await context.Carts.FirstOrDefaultAsync(x => x.TempUserId == tempUserId);
        if (cart == null)
        {
            return false;
        }

        context.Carts.Remove(cart);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCartItemAsync(string? tempUserId, int userId, int productId, ProductSize size)
    {
        await using var context = new Context();
        Cart? cart = !string.IsNullOrWhiteSpace(tempUserId)
            ? await context.Carts.FirstOrDefaultAsync(x => x.TempUserId == tempUserId)
            : await context.Carts.FirstOrDefaultAsync(x => x.AppUserId == userId);

        if (cart == null)
        {
            return false;
        }

        var cartItem = await context.CartItems.FirstOrDefaultAsync(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
        if (cartItem == null)
        {
            return false;
        }

        context.CartItems.Remove(cartItem);
        await context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(tempUserId))
        {
            await ValidateCartCoupons(tempUserId, 0);
        }
        else
        {
            await ValidateCartCoupons(null, userId);
        }

        return true;
    }

    public async Task<CartDetailDto?> GetCartDetailsAsync(string? tempUserId, int userId)
    {
        await ValidateCartCoupons(tempUserId, userId);

        await using var context = new Context();

        IQueryable<CartDetailDto> query = context.Carts
            .AsNoTracking()
            .Select(c => new CartDetailDto
            {
                CartId = c.CartId,
                UpdatedDate = c.UpdatedDate,
                AppUserId = c.AppUserId,
                TempUserId = c.TempUserId,
                Items = c.CartItems.Select(ci => new CartItemDetailDto
                {
                    CartItemId = ci.CartItemId,
                    CartId = ci.CartId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    CategoryName = ci.Product.Category.Name,
                    Price = ci.Product.Price,
                    DiscountRate = ci.Product.DiscountRate,
                    DiscountAmount = ci.Product.DiscountAmount,
                    Quantity = ci.Quantity,
                    Size = ci.Size,
                    PrimaryImageUrl = ci.Product.ProductImages
                        .OrderByDescending(pi => pi.IsMain)
                        .ThenBy(pi => pi.ProductImageId)
                        .Select(pi => pi.Url)
                        .FirstOrDefault(),
                    ImageUrls = ci.Product.ProductImages
                        .OrderByDescending(pi => pi.IsMain)
                        .ThenBy(pi => pi.ProductImageId)
                        .Select(pi => pi.Url)
                        .ToList()
                }).ToList(),
                Coupons = c.CartCoupons.Select(cc => new CartCouponDetailDto
                {
                    CouponId = cc.CouponId,
                    Code = cc.Coupon.Code,
                    DiscountAmount = cc.Coupon.DiscountAmount,
                    DiscountPercentage = cc.Coupon.DiscountPercentage,
                    ExpirationDate = cc.Coupon.ExpirationDate,
                    IsActive = cc.Coupon.IsActive
                }).ToList()
            });

        if (!string.IsNullOrWhiteSpace(tempUserId))
        {
            return await query.FirstOrDefaultAsync(x => x.TempUserId == tempUserId);
        }

        return await query.FirstOrDefaultAsync(x => x.AppUserId == userId);
    }

    public async Task ValidateCartCoupons(string? tempUserId, int userId)
    {
        await using var context = new Context();

        var cartQuery = context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .Include(c => c.CartCoupons)
                .ThenInclude(cc => cc.Coupon);

        Cart? cart = !string.IsNullOrWhiteSpace(tempUserId)
            ? await cartQuery.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
            : await cartQuery.FirstOrDefaultAsync(c => c.AppUserId == userId);

        if (cart == null || cart.CartCoupons == null || !cart.CartCoupons.Any())
        {
            return;
        }

        var cartTotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
        var couponsToRemove = new List<CartCoupon>();

        if (cart.CartCoupons.Count > 1)
        {
            couponsToRemove.AddRange(cart.CartCoupons.Skip(1));
        }

        foreach (var cartCoupon in cart.CartCoupons)
        {
            var coupon = cartCoupon.Coupon;

            if (coupon.MinOrderAmount.HasValue && cartTotalPrice < coupon.MinOrderAmount.Value)
            {
                couponsToRemove.Add(cartCoupon);
                continue;
            }

            if (!coupon.IsActive || coupon.ExpirationDate < DateTime.Now)
            {
                couponsToRemove.Add(cartCoupon);
                continue;
            }

            if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
            {
                couponsToRemove.Add(cartCoupon);
            }
        }

        if (couponsToRemove.Any())
        {
            context.CartCoupons.RemoveRange(couponsToRemove);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> RemoveCouponFromCart(string? tempUserId, int userId, string couponCode)
    {
        await using var context = new Context();

        var coupon = await context.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode);
        if (coupon == null)
        {
            return false;
        }

        Cart? cart = !string.IsNullOrWhiteSpace(tempUserId)
            ? await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
            : await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

        if (cart == null)
        {
            return false;
        }

        var cartCoupon = cart.CartCoupons?.FirstOrDefault(cc => cc.CouponId == coupon.CouponId);
        if (cartCoupon == null)
        {
            return false;
        }

        context.CartCoupons.Remove(cartCoupon);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferCartAsync(string tempUserId, int appUserId)
    {
        await using var context = new Context();

        var tempCart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(x => x.TempUserId == tempUserId);
        var userCart = await context.Carts.FirstOrDefaultAsync(x => x.AppUserId == appUserId);

        if (tempCart == null || userCart == null)
        {
            return false;
        }

        foreach (var item in tempCart.CartItems.ToList())
        {
            item.CartId = userCart.CartId;
        }

        await context.SaveChangesAsync();
        return true;
    }
}
