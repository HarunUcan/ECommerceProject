using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.EntityLayer.Concrete
{
    public class Basket
    {
        public int BasketId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<BasketCoupon>? BasketCoupons { get; set; }
        public ICollection<BasketItem>? BasketItems { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string? TempUserId { get; set; }
    }
}
