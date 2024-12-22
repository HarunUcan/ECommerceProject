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

        public void TDelete(Cart t)
        {
            _cartDal.Delete(t);
        }

        public Cart TGetById(int id)
        {
            return _cartDal.GetById(id);
        }

        public List<Cart> TGetList()
        {
            return _cartDal.GetList();
        }

        public void TInsert(Cart t)
        {
            _cartDal.Insert(t);
        }

        public void TUpdate(Cart t)
        {
            _cartDal.Update(t);
        }
    }
}
