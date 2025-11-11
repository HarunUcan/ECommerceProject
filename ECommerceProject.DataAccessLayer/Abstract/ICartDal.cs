using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.DataAccessLayer.Abstract;

public interface ICartDal : IGenericDal<Cart>
{
    Task<bool> DeleteByTempUserIdAsync(string tempUserId);
    Task<bool> TransferCartAsync(string tempUserId, int appUserId);
    Task<bool> AddToCartAsync(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
    Task<CartDetailDto?> GetCartDetailsAsync(string? tempUserId, int userId);
    Task<bool> DeleteCartItemAsync(string? tempUserId, int userId, int productId, ProductSize size);
    Task<bool> ApplyCoupon(string? tempUserId, int userId, string couponCode);
    Task<bool> RemoveCouponFromCart(string? tempUserId, int userId, string couponCode);
    Task ValidateCartCoupons(string? tempUserId, int userId);
}
