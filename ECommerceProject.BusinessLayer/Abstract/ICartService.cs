using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.BusinessLayer.Abstract;

public interface ICartService : IGenericService<Cart>
{
    Task<bool> TDeleteByTempUserIdAsync(string tempUserId);
    Task<bool> TTransferCartAsync(string tempUserId, int appUserId);
    Task<bool> TAddToCartAsync(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
    Task<CartDetailDto?> TGetCartDetailsAsync(string? tempUserId, int userId);
    Task<bool> TDeleteCartItemAsync(string? tempUserId, int userId, int productId, ProductSize size);
    Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode);
    Task<bool> TRemoveCouponFromCart(string? tempUserId, int userId, string couponCode);
    Task TValidateCartCoupons(string? tempUserId, int userId);
}
