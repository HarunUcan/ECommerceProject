using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface ICartDal : IGenericDal<Cart>
    {
        bool DeleteByTempUserId(string tempUserId);
        bool TransferCart(string tempUserId, int appUserId);
        bool AddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
        Cart GetCart(string? tempUserId, int userId);
        bool DeleteCartItem(string? tempUserId, int userId, int productId, ProductSize size);
        Task<bool> ApplyCoupon(string? tempUserId, int userId, string couponCode);
        Task<bool> RemoveCouponFromCart(string? tempUserId, int userId, string couponCode);
        Task ValidateCartCoupons(string? tempUserId, int userId);
    }
}
