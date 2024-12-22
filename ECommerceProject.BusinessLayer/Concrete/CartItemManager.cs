using ECommerceProject.BusinessLayer.Abstract;
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
        private readonly ICartItemService _cartItemService;

        public CartItemManager(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        public void TDelete(CartItem t)
        {
            _cartItemService.TDelete(t);
        }

        public CartItem TGetById(int id)
        {
            return _cartItemService.TGetById(id);
        }

        public List<CartItem> TGetList()
        {
            return _cartItemService.TGetList();
        }

        public void TInsert(CartItem t)
        {
            _cartItemService.TInsert(t);
        }

        public void TUpdate(CartItem t)
        {
            _cartItemService.TUpdate(t);
        }
    }
}
