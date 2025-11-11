using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.BusinessLayer.Concrete;

public class CartManager : ICartService
{
    private readonly ICartDal _cartDal;

    public CartManager(ICartDal cartDal)
    {
        _cartDal = cartDal;
    }

    public async Task<bool> TAddToCartAsync(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        => await _cartDal.AddToCartAsync(tempUserId, userId, productId, quantity, size);

    public async Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode)
        => await _cartDal.ApplyCoupon(tempUserId, userId, couponCode);

    public void TDelete(Cart t) => _cartDal.Delete(t);

    public async Task<bool> TDeleteByTempUserIdAsync(string tempUserId)
        => await _cartDal.DeleteByTempUserIdAsync(tempUserId);

    public async Task<bool> TDeleteCartItemAsync(string? tempUserId, int userId, int productId, ProductSize size)
        => await _cartDal.DeleteCartItemAsync(tempUserId, userId, productId, size);

    public Cart TGetById(int id) => _cartDal.GetById(id);

    public async Task<CartDetailDto?> TGetCartDetailsAsync(string? tempUserId, int userId)
        => await _cartDal.GetCartDetailsAsync(tempUserId, userId);

    public List<Cart> TGetList() => _cartDal.GetList();

    public void TInsert(Cart t) => _cartDal.Insert(t);

    public async Task<bool> TRemoveCouponFromCart(string? tempUserId, int userId, string couponCode)
        => await _cartDal.RemoveCouponFromCart(tempUserId, userId, couponCode);

    public async Task<bool> TTransferCartAsync(string tempUserId, int appUserId)
        => await _cartDal.TransferCartAsync(tempUserId, appUserId);

    public void TUpdate(Cart t) => _cartDal.Update(t);

    public async Task TValidateCartCoupons(string? tempUserId, int userId)
        => await _cartDal.ValidateCartCoupons(tempUserId, userId);
}
