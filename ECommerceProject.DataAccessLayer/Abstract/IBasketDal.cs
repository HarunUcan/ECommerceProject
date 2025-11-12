using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface IBasketDal : IGenericDal<Basket>
    {
        bool DeleteByTempUserId(string tempUserId);
        bool TransferBasket(string tempUserId, int appUserId);
        bool AddToBasket(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
        Basket GetBasket(string? tempUserId, int userId);
        bool DeleteBasketItem(string? tempUserId, int userId, int productId, ProductSize size);
        Task<bool> ApplyCoupon(string? tempUserId, int userId, string couponCode);
        Task<bool> RemoveCouponFromBasket(string? tempUserId, int userId, string couponCode);
        Task ValidateBasketCoupons(string? tempUserId, int userId);
    }
}
