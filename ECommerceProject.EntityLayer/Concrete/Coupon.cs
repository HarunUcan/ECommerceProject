using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; }
    }
}
