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
    public class CartManager : ICartService
    {
        private readonly ICartDal _cartDal;

        public CartManager(ICartDal cartDal)
        {
            _cartDal = cartDal;
        }

        public bool TAddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        {
            return _cartDal.AddToCart(tempUserId, userId, productId, quantity, size);
        }

        public void TDelete(Cart t)
        {
            _cartDal.Delete(t);
        }

        public bool TDeleteByTempUserId(string tempUserId)
        {
            return _cartDal.DeleteByTempUserId(tempUserId);
        }

        public bool TDeleteCartItem(string? tempUserId, int userId, int productId, ProductSize size)
        {
            return _cartDal.DeleteCartItem(tempUserId, userId, productId, size);
        }

        public Cart TGetById(int id)
        {
            return _cartDal.GetById(id);
        }

        public Cart TGetCart(string? tempUserId, int userId)
        {
            return _cartDal.GetCart(tempUserId, userId);
        }

        public List<Cart> TGetList()
        {
            return _cartDal.GetList();
        }

        public void TInsert(Cart t)
        {
            _cartDal.Insert(t);
        }

        public bool TTransferCart(string tempUserId, int appUserId)
        {
            return _cartDal.TransferCart(tempUserId, appUserId);
        }

        public void TUpdate(Cart t)
        {
            _cartDal.Update(t);
        }
    }
}
