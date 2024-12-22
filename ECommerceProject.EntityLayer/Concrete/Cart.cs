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
        public string? DiscountCode { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public AppUser AppUser { get; set; }
    }
}
