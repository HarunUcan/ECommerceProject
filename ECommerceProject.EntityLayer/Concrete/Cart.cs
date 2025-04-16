using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Cart
    {
        public int CartId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<CartCoupon>? CartCoupons { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? TempUserId { get; set; }
    }
}
