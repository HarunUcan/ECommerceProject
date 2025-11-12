using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class BasketCoupon
    {
        public int BasketCouponId { get; set; }
        public int BasketId { get; set; }
        public Basket? Basket { get; set; }
        public int CouponId { get; set; }
        public Coupon? Coupon { get; set; }
    }
}
