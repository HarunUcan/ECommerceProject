using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfCartDal : GenericRepository<Cart>, ICartDal
    {
        public bool AddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        {
            using (Context context = new())
            {
                if(tempUserId != null)
                {
                    var cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            TempUserId = tempUserId
                        };
                        context.Carts.Add(cart);
                        context.SaveChanges();
                    }
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity += quantity;
                    }
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    var cart = context.Carts.FirstOrDefault(x => x.AppUserId == userId);
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            AppUserId = userId
                        };
                        context.Carts.Add(cart);
                        context.SaveChanges();
                    }
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity += quantity;
                    }
                    context.SaveChanges();
                    return true;
                }
            }
        }

        public async Task<bool> ApplyCoupon(string? tempUserId, int userId, string couponCode)
        {
            using (Context context = new())
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode);
                if (coupon == null) return false; // Coupon not found

                if(!coupon.IsActive) return false; // Coupon is not active

                Cart cart;
                if (tempUserId != null)
                    cart = await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.TempUserId == tempUserId);

                else
                    cart = await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (cart == null) return false;

                // Sepette aynı kupon zaten var mı kontrol et
                bool alreadyApplied = cart.CartCoupons?.Any(cc => cc.CouponId == coupon.CouponId) ?? false;
                if (alreadyApplied) return false;
                // Kuponu sepete ekle
                var cartCoupon = new CartCoupon
                {
                    CartId = cart.CartId,
                    CouponId = coupon.CouponId
                };
                context.CartCoupons.Add(cartCoupon);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public bool DeleteByTempUserId(string tempUserId)
        {
            using (Context context = new())
            {
                var cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                if (cart != null)
                {
                    context.Carts.Remove(cart);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool DeleteCartItem(string? tempUserId, int userId, int productId, ProductSize size)
        {
            using (Context context = new())
            {
                Cart cart;
                if (tempUserId != null)
                {
                    cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    cart = context.Carts.FirstOrDefault(x => x.AppUserId == userId);
                }

                if (cart != null)
                {
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem != null)
                    {
                        context.CartItems.Remove(cartItem);
                        context.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
        }

        public Cart GetCart(string? tempUserId, int userId)
        {

            using (Context context = new())
            {
                IQueryable<Cart> query = context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductVariants)
                    .Include(c => c.CartCoupons) // Kupon ilişkisini getir
                        .ThenInclude(cc => cc.Coupon); // Kuponun kendisini de getir

                if (tempUserId != null)
                {
                    return query.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    return query.FirstOrDefault(x => x.AppUserId == userId);
                }
            }

            //using (Context context = new())
            //{
            //    if (tempUserId != null)
            //    {
            //        return context.Carts
            //            .Include(c => c.CartItems)
            //                .ThenInclude(ci => ci.Product) // CartItem içindeki Product'ı getir
            //                    .ThenInclude(p => p.ProductImages) // Product içindeki resimleri getir
            //            .Include(c => c.CartItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductVariants) // Varyantları da getir
            //            .FirstOrDefault(x => x.TempUserId == tempUserId);
            //    }
            //    else
            //    {
            //        return context.Carts
            //            .Include(c => c.CartItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductImages)
            //            .Include(c => c.CartItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductVariants)
            //            .FirstOrDefault(x => x.AppUserId == userId);
            //    }
            //}

        }

        public async Task<bool> RemoveCouponFromCart(string? tempUserId, int userId, string couponCode)
        {
            using (Context context = new())
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode);
                if (coupon == null) return false; // Kupon bulunamadı

                Cart cart;
                if (tempUserId != null)
                    cart = await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.TempUserId == tempUserId);

                else
                    cart = await context.Carts
                .Include(c => c.CartCoupons)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (cart == null) return false;

                // Sepette aynı kupon zaten var mı kontrol et
                var cartCoupon = cart.CartCoupons?.FirstOrDefault(cc => cc.CouponId == coupon.CouponId);
                if (cartCoupon == null) return false;

                // Kuponu sepetten çıkar
                context.CartCoupons.Remove(cartCoupon);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public bool TransferCart(string tempUserId, int appUserId)
        {
            using (Context context = new())
            {
                var tempCart = context.Carts.Include(c => c.CartItems).FirstOrDefault(x => x.TempUserId == tempUserId);
                var userCart = context.Carts.FirstOrDefault(x => x.AppUserId == appUserId);

                if(tempCart == null || userCart == null) return false;
                
                if (tempCart != null)
                {
                    List<CartItem> tempCartItems = tempCart.CartItems.ToList();
                    if(tempCartItems != null)
                    {
                        foreach (var item in tempCartItems)
                        {
                            item.CartId = userCart.CartId;
                        }
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }
    }
}
