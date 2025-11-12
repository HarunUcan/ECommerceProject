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
    public class EfBasketDal : GenericRepository<Basket>, IBasketDal
    {
        public bool AddToBasket(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        {
            using (Context context = new())
            {
                if(tempUserId != null)
                {
                    var basket = context.Baskets.FirstOrDefault(x => x.TempUserId == tempUserId);
                    if (basket == null)
                    {
                        basket = new Basket
                        {
                            TempUserId = tempUserId
                        };
                        context.Baskets.Add(basket);
                        context.SaveChanges();
                    }
                    var basketItem = context.BasketItems.FirstOrDefault(x => x.BasketId == basket.BasketId && x.ProductId == productId && x.Size == size);
                    if (basketItem == null)
                    {
                        basketItem = new BasketItem
                        {
                            BasketId = basket.BasketId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.BasketItems.Add(basketItem);
                    }
                    else
                    {
                        basketItem.Quantity += quantity;
                    }
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    var basket = context.Baskets.FirstOrDefault(x => x.AppUserId == userId);
                    if (basket == null)
                    {
                        basket = new Basket
                        {
                            AppUserId = userId
                        };
                        context.Baskets.Add(basket);
                        context.SaveChanges();
                    }
                    var basketItem = context.BasketItems.FirstOrDefault(x => x.BasketId == basket.BasketId && x.ProductId == productId && x.Size == size);
                    if (basketItem == null)
                    {
                        basketItem = new BasketItem
                        {
                            BasketId = basket.BasketId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.BasketItems.Add(basketItem);
                    }
                    else
                    {
                        basketItem.Quantity += quantity;
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

                // Kullanıcı giriş yapmamışsa indirim kullanamaz
                if (tempUserId != null)
                {
                    throw new Exception("İndirimlerden yararlanmak için kayıt olmalı ve giriş yapmalısınız!"); //return false; // Geçici kullanıcılar kupon kullanamaz
                }

                var query = context.Baskets
                                .Include(c => c.BasketItems)
                                    .ThenInclude(ci => ci.Product)
                                .Include(c => c.BasketCoupons)
                                    .ThenInclude(cc => cc.Coupon);

                Basket? basket = tempUserId != null
                    ? await query.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
                    : await query.FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (basket == null) return false;


                // Son Kullanım tarihini kontrol et
                if (coupon.ExpirationDate < DateTime.Now)
                {
                    throw new Exception("Bu kuponun son kullanma tarihi geçmiştir!"); //return false; // Kuponun son kullanma tarihi geçmiş
                }

                // Sepette aynı kupon zaten var mı kontrol et
                bool alreadyApplied = basket.BasketCoupons?.Any(cc => cc.CouponId == coupon.CouponId) ?? false;
                if (alreadyApplied)
                {
                    throw new Exception("Bu kupon zaten kullanılıyor!"); //return false; // Kupon zaten sepette var
                }

                bool basketHasAnyCoupons = basket.BasketCoupons?.Any() ?? false;
                // Eğer sepette başka kupon varsa yeni kupon eklenemez
                if (basketHasAnyCoupons)
                {
                    throw new Exception("Sepette yalnızca 1 kupon kullanabilirsiniz!"); //return false; // Sepette zaten kupon var
                }


                //bool isCouponTypePercentage = coupon.DiscountPercentage != null && coupon.DiscountPercentage > 0;
                //// Sepette zaten yüzdelik kupon var mı kontrol et
                //if (isCouponTypePercentage && basket.BasketCoupons.Any(cc => cc.Coupon.DiscountPercentage != null && cc.Coupon.DiscountPercentage > 0))
                //{
                //    throw new Exception("Sepette maksimum 1 adet yüzdelik indirim sağlayan kupon olabilir!"); //return false; // Yüzde indirimli kupon zaten var
                //}

                // Kuponun kullanım sayısını kontrol et
                if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
                {
                    throw new Exception("Bu kuponun kullanım limiti dolmuştur!"); //return false; // Kuponun kullanım limiti dolmuş
                }
                // Kuponun minimum sipariş tutarını kontrol et
                var basketTotalPrice = basket.BasketItems?.Sum(ci => ci.Quantity * ci.Product.Price);
                if (basketTotalPrice < coupon.MinOrderAmount)
                {
                    throw new Exception("Bu kuponu kullanmak için olan minimum sipariş tutarı sağlanmamıştır!"); //return false; // Minimum sipariş tutarı sağlanmamış
                }
                
                // Kuponu sepete ekle
                var basketCoupon = new BasketCoupon
                {
                    BasketId = basket.BasketId,
                    CouponId = coupon.CouponId
                };
                context.BasketCoupons.Add(basketCoupon);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public bool DeleteByTempUserId(string tempUserId)
        {
            using (Context context = new())
            {
                var basket = context.Baskets.FirstOrDefault(x => x.TempUserId == tempUserId);
                if (basket != null)
                {
                    context.Baskets.Remove(basket);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool DeleteBasketItem(string? tempUserId, int userId, int productId, ProductSize size)
        {
            using (Context context = new())
            {
                Basket basket;
                if (tempUserId != null)
                {
                    basket = context.Baskets.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    basket = context.Baskets.FirstOrDefault(x => x.AppUserId == userId);
                }

                if (basket != null)
                {
                    var basketItem = context.BasketItems.FirstOrDefault(x => x.BasketId == basket.BasketId && x.ProductId == productId && x.Size == size);
                    if (basketItem != null)
                    {
                        context.BasketItems.Remove(basketItem);
                        context.SaveChanges();

                        // Sepetteki kuponları kontrol et ve geçersiz olanları kaldır
                        if (tempUserId != null)
                            ValidateBasketCoupons(tempUserId, 0).Wait();
                        else
                            ValidateBasketCoupons(null, userId).Wait();

                        return true;
                    }
                }
                return false;
            }
        }

        public Basket GetBasket(string? tempUserId, int userId)
        {

            using (Context context = new())
            {
                IQueryable<Basket> query = context.Baskets
                    .Include(c => c.BasketItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductImages)
                    .Include(c => c.BasketItems)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.ProductVariants)
                    .Include(c => c.BasketCoupons) // Kupon ilişkisini getir
                        .ThenInclude(cc => cc.Coupon); // Kuponun kendisini de getir

                if (tempUserId != null)
                {
                    ValidateBasketCoupons(tempUserId, 0).Wait();
                    return query.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    ValidateBasketCoupons(null, userId).Wait();
                    return query.FirstOrDefault(x => x.AppUserId == userId);
                }
            }

            //using (Context context = new())
            //{
            //    if (tempUserId != null)
            //    {
            //        return context.Baskets
            //            .Include(c => c.BasketItems)
            //                .ThenInclude(ci => ci.Product) // BasketItem içindeki Product'ı getir
            //                    .ThenInclude(p => p.ProductImages) // Product içindeki resimleri getir
            //            .Include(c => c.BasketItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductVariants) // Varyantları da getir
            //            .FirstOrDefault(x => x.TempUserId == tempUserId);
            //    }
            //    else
            //    {
            //        return context.Baskets
            //            .Include(c => c.BasketItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductImages)
            //            .Include(c => c.BasketItems)
            //                .ThenInclude(ci => ci.Product)
            //                    .ThenInclude(p => p.ProductVariants)
            //            .FirstOrDefault(x => x.AppUserId == userId);
            //    }
            //}

        }

        // Sepetteki kuponları kontrol et ve geçersiz olanları kaldır
        public async Task ValidateBasketCoupons(string? tempUserId, int userId)
        {
            using (Context context = new())
            {
                var basketQuery = context.Baskets
                    .Include(c => c.BasketItems)
                        .ThenInclude(ci => ci.Product)
                    .Include(c => c.BasketCoupons)
                        .ThenInclude(cc => cc.Coupon);

                Basket? basket = tempUserId != null
                    ? await basketQuery.FirstOrDefaultAsync(c => c.TempUserId == tempUserId)
                    : await basketQuery.FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (basket == null || basket.BasketCoupons == null || !basket.BasketCoupons.Any())
                    return;

                var basketTotalPrice = basket.BasketItems.Sum(ci => ci.Quantity * ci.Product.Price);

                var couponsToRemove = new List<BasketCoupon>();

                // Sepette yalnızca 1 kupon olabilir
                if(basket.BasketCoupons.Count > 1)
                {
                    couponsToRemove.AddRange(basket.BasketCoupons.Skip(1)); // İlk kuponu bırak, diğerlerini kaldır
                }

                foreach (var basketCoupon in basket.BasketCoupons)
                {
                    var coupon = basketCoupon.Coupon;

                    // Minimum sipariş tutarı şartını sağlamıyor
                    if (coupon.MinOrderAmount.HasValue && basketTotalPrice < coupon.MinOrderAmount.Value)
                    {
                        couponsToRemove.Add(basketCoupon);
                        continue;
                    }

                    // Expired ya da artık aktif değil
                    if (!coupon.IsActive || coupon.ExpirationDate < DateTime.Now)
                    {
                        couponsToRemove.Add(basketCoupon);
                        continue;
                    }

                    // Kullanım limiti dolmuş
                    if (coupon.CurrentUsageCount >= coupon.MaxUsageCount)
                    {
                        couponsToRemove.Add(basketCoupon);
                        continue;
                    }

                    // Eğer bu kupon yüzdelikse ve sepette başka yüzdelik varsa (gerekiyorsa)
                    //if (coupon.DiscountPercentage.HasValue)
                    //{
                    //    bool hasOtherPercentage = basket.BasketCoupons
                    //        .Where(cc => cc.CouponId != coupon.CouponId)
                    //        .Any(cc => cc.Coupon.DiscountPercentage.HasValue);

                    //    if (hasOtherPercentage)
                    //    {
                    //        couponsToRemove.Add(basketCoupon);
                    //        continue;
                    //    }
                    //}
                }

                if (couponsToRemove.Any())
                {
                    context.BasketCoupons.RemoveRange(couponsToRemove);
                    await context.SaveChangesAsync();
                }
            }
        }


        public async Task<bool> RemoveCouponFromBasket(string? tempUserId, int userId, string couponCode)
        {
            using (Context context = new())
            {
                var coupon = await context.Coupons.FirstOrDefaultAsync(x => x.Code == couponCode);
                if (coupon == null) return false; // Kupon bulunamadı

                Basket basket;
                if (tempUserId != null)
                    basket = await context.Baskets
                .Include(c => c.BasketCoupons)
                .FirstOrDefaultAsync(c => c.TempUserId == tempUserId);

                else
                    basket = await context.Baskets
                .Include(c => c.BasketCoupons)
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (basket == null) return false;

                // Sepette aynı kupon zaten var mı kontrol et
                var basketCoupon = basket.BasketCoupons?.FirstOrDefault(cc => cc.CouponId == coupon.CouponId);
                if (basketCoupon == null) return false;

                // Kuponu sepetten çıkar
                context.BasketCoupons.Remove(basketCoupon);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public bool TransferBasket(string tempUserId, int appUserId)
        {
            using (Context context = new())
            {
                var tempBasket = context.Baskets.Include(c => c.BasketItems).FirstOrDefault(x => x.TempUserId == tempUserId);
                var userBasket = context.Baskets.FirstOrDefault(x => x.AppUserId == appUserId);

                if(tempBasket == null || userBasket == null) return false;
                
                if (tempBasket != null)
                {
                    List<BasketItem> tempBasketItems = tempBasket.BasketItems.ToList();
                    if(tempBasketItems != null)
                    {
                        foreach (var item in tempBasketItems)
                        {
                            item.BasketId = userBasket.BasketId;
                        }
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }
    }
}
