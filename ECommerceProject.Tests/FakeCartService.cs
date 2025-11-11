using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.Tests;

public class FakeCartService : ICartService
{
    private readonly Dictionary<string, Cart> _tempCarts = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> TempUserIds => _tempCarts.Keys.ToList();

    public void Clear() => _tempCarts.Clear();

    public void SeedTempCart(string tempUserId)
    {
        _tempCarts[tempUserId] = new Cart
        {
            TempUserId = tempUserId,
            UpdatedDate = DateTime.UtcNow
        };
    }

    public bool TDeleteByTempUserId(string tempUserId) => _tempCarts.Remove(tempUserId);

    public void TInsert(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts[t.TempUserId] = t;
        }
    }

    public void TDelete(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts.Remove(t.TempUserId);
        }
    }

    public void TUpdate(Cart t)
    {
        if (!string.IsNullOrWhiteSpace(t.TempUserId))
        {
            _tempCarts[t.TempUserId] = t;
        }
    }

    public List<Cart> TGetList() => _tempCarts.Values.ToList();

    public Cart TGetById(int id) => throw new NotSupportedException();

    public bool TTransferCart(string tempUserId, int appUserId) => throw new NotSupportedException();

    public bool TAddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size) => throw new NotSupportedException();

    public Cart TGetCart(string? tempUserId, int userId)
    {
        if (!string.IsNullOrWhiteSpace(tempUserId) && _tempCarts.TryGetValue(tempUserId, out var cart))
        {
            return cart;
        }

        return new Cart { AppUserId = userId, UpdatedDate = DateTime.UtcNow };
    }

    public bool TDeleteCartItem(string? tempUserId, int userId, int productId, ProductSize size) => throw new NotSupportedException();

    public Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode) => throw new NotSupportedException();

    public Task<bool> TRemoveCouponFromCart(string? tempUserId, int userId, string couponCode) => throw new NotSupportedException();

    public Task TValidateCartCoupons(string? tempUserId, int userId) => Task.CompletedTask;
}
