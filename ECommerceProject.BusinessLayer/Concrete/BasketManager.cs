using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class BasketManager : IBasketService
    {
        private readonly IBasketDal _basketDal;

        public BasketManager(IBasketDal basketDal)
        {
            _basketDal = basketDal;
        }

        public bool TAddToBasket(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        {
            return _basketDal.AddToBasket(tempUserId, userId, productId, quantity, size);
        }

        public async Task<bool> TApplyCoupon(string? tempUserId, int userId, string couponCode)
        {
            return await _basketDal.ApplyCoupon(tempUserId, userId, couponCode);
        }

        public void TDelete(Basket t)
        {
            _basketDal.Delete(t);
        }

        public bool TDeleteByTempUserId(string tempUserId)
        {
            return _basketDal.DeleteByTempUserId(tempUserId);
        }

        public bool TDeleteBasketItem(string? tempUserId, int userId, int productId, ProductSize size)
        {
            return _basketDal.DeleteBasketItem(tempUserId, userId, productId, size);
        }

        public Basket TGetById(int id)
        {
            return _basketDal.GetById(id);
        }

        public Basket TGetBasket(string? tempUserId, int userId)
        {
            return _basketDal.GetBasket(tempUserId, userId);
        }

        public List<Basket> TGetList()
        {
            return _basketDal.GetList();
        }

        public void TInsert(Basket t)
        {
            _basketDal.Insert(t);
        }

        public async Task<bool> TRemoveCouponFromBasket(string? tempUserId, int userId, string couponCode)
        {
            return await _basketDal.RemoveCouponFromBasket(tempUserId, userId, couponCode);
        }

        public bool TTransferBasket(string tempUserId, int appUserId)
        {
            return _basketDal.TransferBasket(tempUserId, appUserId);
        }

        public void TUpdate(Basket t)
        {
            _basketDal.Update(t);
        }

        public async Task TValidateBasketCoupons(string? tempUserId, int userId)
        {
            await _basketDal.ValidateBasketCoupons(tempUserId, userId);
        }
    }
}
