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
    public class CartItemManager : ICartItemService
    {
        private readonly ICartItemDal _cartItemDal;

        public CartItemManager(ICartItemDal cartItemDal)
        {
            _cartItemDal = cartItemDal;
        }

        public void TDelete(CartItem t)
        {
            _cartItemDal.Delete(t);
        }

        public CartItem TGetById(int id)
        {
            return _cartItemDal.GetById(id);
        }

        public List<CartItem> TGetList()
        {
            return _cartItemDal.GetList();
        }

        public void TInsert(CartItem t)
        {
            _cartItemDal.Insert(t);
        }

        public void TUpdate(CartItem t)
        {
            _cartItemDal.Update(t);
        }
    }
}
