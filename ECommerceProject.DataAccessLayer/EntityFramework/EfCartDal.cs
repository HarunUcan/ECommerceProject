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
                if (coupon == null)
                {
                    throw new Exception("Bu kupon kodu geçersiz!"); //return false; // Kupon bulunamadı
                }

                if (!coupon.IsActive)
                {
                    throw new Exception("Bu kupon kodu geçersiz!"); //return false; // Kupon aktif değil
                }

                var query = context.Carts
                                .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Product)
                                .Include(c => c.CartCoupons)
                                    .ThenInclude(cc => cc.Coupon);

                Cart? cart = tempUserId != null
                    ? await query.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
                    : await query.FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (cart == null) return false;


                // Son Kullanım tarihini kontrol et
                if (coupon.ExpirationDate < DateTime.Now)
                {
                    throw new Exception("Bu kuponun son kullanma tarihi geçmiştir!"); //return false; // Kuponun son kullanma tarihi geçmiş
                }

                // Sepette aynı kupon zaten var mı kontrol et
                bool alreadyApplied = cart.CartCoupons?.Any(cc => cc.CouponId == coupon.CouponId) ?? false;
                if (alreadyApplied)
                {
                    throw new Exception("Bu kupon zaten kullanılıyor!"); //return false; // Kupon zaten sepette var
                }

                bool isCouponTypePercentage = coupon.DiscountPercentage != null && coupon.DiscountPercentage > 0;
                // Sepette zaten yüzdelik kupon var mı kontrol et
                if (isCouponTypePercentage && cart.CartCoupons.Any(cc => cc.Coupon.DiscountPercentage != null && cc.Coupon.DiscountPercentage > 0))
                {
                    throw new Exception("Sepette maksimum 1 adet yüzdelik indirim sağlayan kupon olabilir!"); //return false; // Yüzde indirimli kupon zaten var
                }

                // Kuponun kullanım sayısını kontrol et
                if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
                {
                    throw new Exception("Bu kuponun kullanım limiti dolmuştur!"); //return false; // Kuponun kullanım limiti dolmuş
                }
                // Kuponun minimum sipariş tutarını kontrol et
                var cartTotalPrice = cart.CartItems?.Sum(ci => ci.Quantity * ci.Product.Price);
                if (cartTotalPrice < coupon.MinOrderAmount)
                {
                    throw new Exception("Bu kuponu kullanmak için olan minimum sipariş tutarı sağlanmamıştır!"); //return false; // Minimum sipariş tutarı sağlanmamış
                }
                
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

                        // Sepetteki kuponları kontrol et ve geçersiz olanları kaldır
                        if (tempUserId != null)
                            ValidateCartCoupons(tempUserId, 0).Wait();
                        else
                            ValidateCartCoupons(null, userId).Wait();

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
                    ValidateCartCoupons(tempUserId, 0).Wait();
                    return query.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    ValidateCartCoupons(null, userId).Wait();
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

        // Sepetteki kuponları kontrol et ve geçersiz olanları kaldır
        public async Task ValidateCartCoupons(string? tempUserId, int userId)
        {
            using (Context context = new())
            {
                var cartQuery = context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .Include(c => c.CartCoupons)
                        .ThenInclude(cc => cc.Coupon);

                Cart? cart = tempUserId != null
                    ? await cartQuery.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
                    : await cartQuery.FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (cart == null || cart.CartCoupons == null || !cart.CartCoupons.Any())
                    return;

                var cartTotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

                var couponsToRemove = new List<CartCoupon>();

                foreach (var cartCoupon in cart.CartCoupons)
                {
                    var coupon = cartCoupon.Coupon;

                    // Minimum sipariş tutarı şartını sağlamıyor
                    if (coupon.MinOrderAmount.HasValue && cartTotalPrice < coupon.MinOrderAmount.Value)
                    {
                        couponsToRemove.Add(cartCoupon);
                        continue;
                    }

                    // Expired ya da artık aktif değil
                    if (!coupon.IsActive || coupon.ExpirationDate < DateTime.Now)
                    {
                        couponsToRemove.Add(cartCoupon);
                        continue;
                    }

                    // Eğer bu kupon yüzdelikse ve sepette başka yüzdelik varsa (gerekiyorsa)
                    if (coupon.DiscountPercentage.HasValue)
                    {
                        bool hasOtherPercentage = cart.CartCoupons
                            .Where(cc => cc.CouponId != coupon.CouponId)
                            .Any(cc => cc.Coupon.DiscountPercentage.HasValue);

                        if (hasOtherPercentage)
                        {
                            couponsToRemove.Add(cartCoupon);
                            continue;
                        }
                    }
                }

                if (couponsToRemove.Any())
                {
                    context.CartCoupons.RemoveRange(couponsToRemove);
                    await context.SaveChangesAsync();
                }
            }
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
