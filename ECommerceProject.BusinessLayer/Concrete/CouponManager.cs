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
    public class CouponManager : ICouponService
    {
        private readonly ICouponDal _couponDal;

        public CouponManager(ICouponDal couponDal)
        {
            _couponDal = couponDal;
        }

        public void TDelete(Coupon t)
        {
            _couponDal.Delete(t);
        }

        public Coupon TGetById(int id)
        {
            return _couponDal.GetById(id);
        }

        public List<Coupon> TGetList()
        {
            return _couponDal.GetList();
        }

        public void TInsert(Coupon t)
        {
            _couponDal.Insert(t);
        }

        public void TUpdate(Coupon t)
        {
            _couponDal.Update(t);
        }
    }
}
