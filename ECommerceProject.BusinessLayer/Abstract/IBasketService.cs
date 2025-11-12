using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IBasketService : IGenericService<Basket>
    {
        bool TDeleteByTempUserId(string tempUserId);
        bool TTransferBasket(string tempUserId, int appUserId);
        bool TAddToBasket(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
        Basket TGetBasket(string? tempUserId, int userId);
        bool TDeleteBasketItem(string? tempUserId, int userId, int productId, ProductSize size);
        Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode);
        Task<bool> TRemoveCouponFromBasket(string? tempUserId, int userId, string couponCode);
        Task TValidateBasketCoupons(string? tempUserId, int userId);
    }
}
