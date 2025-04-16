using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class CartCoupon
    {
        public int CartCouponId { get; set; }
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public int CouponId { get; set; }
        public Coupon? Coupon { get; set; }
    }
}
